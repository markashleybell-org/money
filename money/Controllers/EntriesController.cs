using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Money.Entities;
using Money.Models;
using Money.Support;
using static Money.Support.MvcExtensions;

namespace Money.Controllers
{
    public class EntriesController : MoneyControllerBase
    {
        private readonly ViewRenderer _viewRenderer;

        private readonly Func<int, Func<IEnumerable<Account>>> _accountList = null;

        public EntriesController(
            IOptionsMonitor<Settings> optionsMonitor,
            IDateTimeService dateTimeService,
            IUnitOfWork unitOfWork,
            IQueryHelper db,
            ViewRenderer viewRenderer)
            : base(
                  optionsMonitor,
                  dateTimeService,
                  unitOfWork,
                  db)
        {
            _viewRenderer = viewRenderer;

            var sql = @"SELECT
                            *
                        FROM
                            Accounts
                        WHERE
                            ID != @AccountID
                        ORDER BY
                            DisplayOrder";

            _accountList = accountID => () => Db.Query(conn => conn.Query<Account>(sql, new { accountID }));
        }

        public IActionResult Index(int id)
        {
            var sql = @"SELECT
                            e.ID,
                            a.Name AS Account,
                            e.Date,
                            p.Name AS Party,
                            c.Name AS Category,
                            e.Amount
                        FROM
                            Entries e
                        INNER JOIN
                            Accounts a ON a.ID = e.AccountID
                        LEFT JOIN
                            Parties p ON p.ID = e.PartyID
                        LEFT JOIN
                            Categories c ON c.ID = e.CategoryID
                        WHERE
                            e.AccountID = @ID
                        ORDER BY
                            e.Date DESC,
                            e.ID DESC";

            var entries = Db.Query(conn => conn.Query<ListEntriesEntryViewModel>(sql, new { id }));

            return View(new ListEntriesViewModel {
                Entries = entries.GroupBy(e => e.Date)
            });
        }

        public IActionResult Create(
            int accountID,
            int? categoryID = null,
            bool? isCredit = null,
            bool showCategorySelector = true,
            decimal remaining = 0)
        {
            var types = EntryType.Debit | EntryType.Credit | EntryType.Transfer;

            if (isCredit.HasValue)
            {
                types = !isCredit.Value
                    ? EntryType.Debit | EntryType.Transfer
                    : EntryType.Credit;
            }

            var typeSelectListItems = TypesSelectListItems(types, _accountList(accountID));

            return View(new CreateEntryViewModel {
                AccountID = accountID,
                CategoryID = categoryID,
                Types = typeSelectListItems,
                Type = typeSelectListItems.Count() == 1 ? typeSelectListItems.Single().Value : null,
                MonthlyBudgets = MonthlyBudgetsSelectListItems(accountID),
                Categories = CategoriesSelectListItems(accountID),
                Parties = PartiesSelectListItems(accountID),
                IsCredit = isCredit,
                ShowCategorySelector = showCategorySelector,
                Remaining = remaining
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { ok = false, msg = "Invalid form values" });
            }

            var ids = new int[0];

            var amount = Math.Abs(model.Amount.Value);

            var monthlyBudgetID = GetLatestMonthlyBudget(model.AccountID);

            if (!Enum.TryParse<EntryType>(model.Type, out var entryType))
            {
                entryType = EntryType.Transfer;
            }

            if (entryType == EntryType.Transfer)
            {
                if (!int.TryParse(model.Type.Split('-')[1], out var destinationAccountID))
                {
                    return Json(new { ok = false, msg = "Invalid destination account ID" });
                }

                var parameters = new { ids = new[] { model.AccountID, destinationAccountID } };

                var accounts = Db.Query(conn => conn.Query<Account>("SELECT * FROM Accounts WHERE ID IN @ids", parameters));
                var sourceAccountName = accounts.Single(a => a.ID == model.AccountID).Name;
                var destinationAccountName = accounts.Single(a => a.ID == destinationAccountID).Name;

                var destinationMonthlyBudgetID = GetLatestMonthlyBudget(destinationAccountID);

                var guid = Guid.NewGuid();

                // For transfers, we set up separate entries for source and destination accounts
                // Note that we ignore the credit/debit selection here (a transfer is always a debit)
                Db.InsertOrUpdate(new Entry(
                    accountID: model.AccountID,
                    monthlyBudgetID: monthlyBudgetID,
                    categoryID: model.CategoryID,
                    date: model.Date,
                    amount: -amount,
                    note: $"Transfer to {destinationAccountName}",
                    transferGuid: guid
                ));

                Db.InsertOrUpdate(new Entry(
                    accountID: destinationAccountID,
                    monthlyBudgetID: destinationMonthlyBudgetID,
                    date: model.Date,
                    amount: amount,
                    note: $"Transfer from {sourceAccountName}",
                    transferGuid: guid
                ));

                ids = new[] { model.AccountID, destinationAccountID };
            }
            else
            {
                if (entryType == EntryType.Debit)
                {
                    amount = -amount;
                }

                Db.InsertOrUpdate(new Entry(
                    accountID: model.AccountID,
                    monthlyBudgetID: monthlyBudgetID,
                    categoryID: model.CategoryID,
                    partyID: model.PartyID,
                    date: model.Date,
                    amount: amount,
                    note: model.Note
                ));

                ids = new[] { model.AccountID };
            }

            UnitOfWork.CommitChanges();

            var getHtmlForUpdatedAccounts = ids.Select(async (id, i) => {
                var html = await RenderAccountHtml(id, i == 0 && !model.CategoryID.HasValue ? 0 : model.CategoryID);
                return new { id, html };
            });

            var updatedData = await Task.WhenAll(getHtmlForUpdatedAccounts);

            return Json(new { ok = true, updated = updatedData });
        }

        public IActionResult Update(int id)
        {
            var dto = Db.Get<Entry>(id);

            return View(new UpdateEntryViewModel {
                ID = dto.ID,
                CategoryID = dto.CategoryID,
                PartyID = dto.PartyID,
                Date = dto.Date,
                Amount = dto.Amount,
                Note = dto.Note,
                Categories = CategoriesSelectListItems(dto.AccountID),
                Parties = PartiesSelectListItems(dto.AccountID)
            });
        }

        [HttpPost]
        public IActionResult Update(UpdateEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = CategoriesSelectListItems(model.ID);
                model.Parties = PartiesSelectListItems(model.ID);

                return View(model);
            }

            var dto = Db.Get<Entry>(model.ID);

            var updated = dto.WithUpdates(
                categoryID: model.CategoryID,
                partyID: model.PartyID,
                date: model.Date,
                amount: model.Amount.Value,
                note: model.Note
            );

            Db.InsertOrUpdate(updated);

            UnitOfWork.CommitChanges();

            return RedirectTo<HomeController>(c => c.Index());
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var dto = Db.Get<Entry>(id);

            Db.Delete(dto);

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index), new { id = dto.AccountID });
        }

        private async Task<string> RenderAccountHtml(int accountID, int? updatedCategoryID = null)
        {
            var parameters = new {
                UserID,
                AccountID = accountID
            };

            var model = Db.Query(conn => {
                using (var reader = conn.QueryMultipleSP("DashboardAccount", parameters))
                {
                    var account = reader.ReadSingle<AccountViewModel>();
                    var categories = reader.Read<CategoryViewModel>();

                    return new AccountViewModel {
                        ID = account.ID,
                        Name = account.Name,
                        CurrentBalance = account.CurrentBalance,
                        Categories = categories,
                        UpdatedCategoryID = updatedCategoryID
                    };
                }
            });

            return await _viewRenderer.Render("_Account", model);
        }

        private int? GetLatestMonthlyBudget(int accountID)
        {
            var sql = "SELECT TOP 1 ID FROM MonthlyBudgets WHERE AccountID = @AccountID AND GETDATE() <= EndDate ORDER BY EndDate DESC, ID DESC";
            return Db.Query(conn => conn.QuerySingleOrDefault<int?>(sql, new { accountID }));
        }

        private IEnumerable<SelectListItem> CategoriesSelectListItems(int accountID) =>
            Db.Query(conn => conn.Query<Category>("SELECT * FROM Categories WHERE AccountID = @AccountID AND Deleted = 0 ORDER BY Name", new { accountID }))
               .Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name });

        private IEnumerable<SelectListItem> PartiesSelectListItems(int accountID) =>
            Db.Query(conn => conn.Query<Party>("SELECT * FROM Parties WHERE AccountID = @AccountID AND Deleted = 0 ORDER BY Name", new { accountID }))
               .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = p.Name });

        private IEnumerable<SelectListItem> MonthlyBudgetsSelectListItems(int accountID) =>
            Db.Query(conn => conn.Query<MonthlyBudget>("SELECT * FROM MonthlyBudgets WHERE AccountID = @AccountID", new { accountID }))
               .Select(b => new SelectListItem { Value = b.ID.ToString(), Text = b.StartDate.ToString("dd/MM/yyyy") + " - " + b.EndDate.ToString("dd/MM/yyyy") });
    }
}
