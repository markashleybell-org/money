using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using money.web.Abstract;
using money.web.Models;
using money.web.Models.Entities;
using money.web.Support;
using static money.web.Support.Extensions;

namespace money.web.Controllers
{
    public class EntriesController : ControllerBase
    {
        public EntriesController(IUnitOfWork unitOfWork, IQueryHelper db, IRequestContext context) : base(unitOfWork, db, context) { }

        public ActionResult Index(int id)
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

            var entries = _db.Query(conn => conn.Query<ListEntriesEntryViewModel>(sql, new { id }));

            return View(new ListEntriesViewModel {
                Entries = entries.GroupBy(e => e.Date)
            });
        }

        public ActionResult Create(int accountID, int? categoryID = null, bool showCategorySelector = true, decimal remaining = 0) => View(new CreateEntryViewModel {
            AccountID = accountID,
            MonthlyBudgetID = GetLatestMonthlyBudget(accountID),
            CategoryID = categoryID,
            Types = TypesSelectListItems(accountID),
            MonthlyBudgets = MonthlyBudgetsSelectListItems(accountID),
            Categories = CategoriesSelectListItems(accountID),
            Parties = PartiesSelectListItems(accountID),
            ShowCategorySelector = showCategorySelector,
            Remaining = remaining
        });

        [HttpPost]
        public ActionResult Create(CreateEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Types = TypesSelectListItems(model.AccountID);
                model.MonthlyBudgets = MonthlyBudgetsSelectListItems(model.AccountID);
                model.Categories = CategoriesSelectListItems(model.AccountID);
                model.Parties = PartiesSelectListItems(model.AccountID);

                return Json(new { ok = false, msg = "Invalid form values" });
            }

            var ids = new int[0];

            var amount = Math.Abs(model.Amount);

            if (!Enum.TryParse<EntryType>(model.Type, out var entryType))
                entryType = EntryType.Transfer;

            if (entryType == EntryType.Transfer)
            {
                if (!int.TryParse(model.Type.Split('-')[1], out var destinationAccountID))
                    return Json(new { ok = false, msg = "Invalid destination account ID" });

                var parameters = new { ids = new[] { model.AccountID, destinationAccountID } };

                var accounts = _db.Query(conn => conn.Query<Account>("SELECT * FROM Accounts WHERE ID IN @ids", parameters));
                var sourceAccountName = accounts.Single(a => a.ID == model.AccountID).Name;
                var destinationAccountName = accounts.Single(a => a.ID == destinationAccountID).Name;

                var destinationMonthlyBudgetID = GetLatestMonthlyBudget(destinationAccountID);

                var guid = Guid.NewGuid();

                // For transfers, we set up separate entries for source and destination accounts
                // Note that we ignore the credit/debit selection here (a transfer is always a debit)

                _db.InsertOrUpdate(new Entry(
                    accountID: model.AccountID,
                    monthlyBudgetID: model.MonthlyBudgetID,
                    categoryID: model.CategoryID,
                    date: model.Date,
                    amount: -amount,
                    note: $"Transfer to {destinationAccountName}",
                    transferGuid: guid
                ));

                _db.InsertOrUpdate(new Entry(
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
                    amount = -amount;

                _db.InsertOrUpdate(new Entry(
                    accountID: model.AccountID,
                    monthlyBudgetID: model.MonthlyBudgetID,
                    categoryID: model.CategoryID,
                    partyID: model.PartyID,
                    date: model.Date,
                    amount: amount,
                    note: model.Note
                ));

                ids = new[] { model.AccountID };
            }

            _unitOfWork.CommitChanges();

            var updated = ids.Select((id, i) => new { id, html = RenderAccountHtml(id, (i == 0 && !model.CategoryID.HasValue ? 0 : model.CategoryID)) });

            return Json(new { ok = true, updated });
        }

        public ActionResult Update(int id)
        {
            var dto = _db.Get<Entry>(id);

            return View(new UpdateEntryViewModel {
                ID = dto.ID,
                AccountID = dto.AccountID,
                MonthlyBudgetID = dto.MonthlyBudgetID,
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
        public ActionResult Update(UpdateEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = CategoriesSelectListItems(model.ID);
                model.Parties = PartiesSelectListItems(model.ID);

                return View(model);
            }

            var dto = _db.Get<Entry>(model.ID);

            var updated = dto.WithUpdates(
                monthlyBudgetID: model.MonthlyBudgetID,
                categoryID: model.CategoryID,
                partyID: model.PartyID,
                date: model.Date,
                amount: model.Amount,
                note: model.Note
            );

            _db.InsertOrUpdate(updated);

            _unitOfWork.CommitChanges();

            return RedirectTo<HomeController>(c => c.Index());
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var dto = _db.Get<Entry>(id);

            _db.Delete(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index), new { id = dto.AccountID });
        }

        private string RenderAccountHtml(int accountID, int? updatedCategoryID = null)
        {
            var model = _db.Query(conn => {
                using (var reader = conn.QueryMultipleSP("Account", new { AccountID = accountID }))
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

            var viewData = new ViewDataDictionary(model);

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, "_Account");
                var viewContext = new ViewContext(ControllerContext, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        private int? GetLatestMonthlyBudget(int accountID)
        {
            var sql = "SELECT TOP 1 ID FROM MonthlyBudgets WHERE AccountID = @AccountID AND GETDATE() <= EndDate ORDER BY EndDate, ID";
            return _db.Query(conn => conn.QuerySingleOrDefault<int?>(sql, new { accountID }));
        }

        private IEnumerable<SelectListItem> TypesSelectListItems(int accountID)
        {
            var accounts = _db.Query(conn => conn.Query<Account>("SELECT * FROM Accounts WHERE ID != @AccountID ORDER BY DisplayOrder", new { accountID }))
               .Select(a => new SelectListItem { Value = $"Transfer-{a.ID}", Text = $"Transfer to {a.Name}" });

            var types = Enum.GetNames(typeof(EntryType)).Where(n => n != EntryType.Transfer.ToString())
                .Select(n => new SelectListItem { Value = n, Text = n }).Concat(accounts);

            return types;
        }

        private IEnumerable<SelectListItem> AccountsSelectListItems() =>
            _db.Query(conn => conn.Query<Account>("SELECT * FROM Accounts ORDER BY DisplayOrder"))
               .Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.Name });

        private IEnumerable<SelectListItem> CategoriesSelectListItems(int accountID) =>
            _db.Query(conn => conn.Query<Category>("SELECT * FROM Categories WHERE AccountID = @AccountID ORDER BY Name", new { accountID }))
               .Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name });

        private IEnumerable<SelectListItem> PartiesSelectListItems(int accountID) =>
            _db.Query(conn => conn.Query<Party>("SELECT * FROM Parties WHERE AccountID = @AccountID ORDER BY Name", new { accountID }))
               .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = p.Name });

        private IEnumerable<SelectListItem> MonthlyBudgetsSelectListItems(int accountID) =>
            _db.Query(conn => conn.Query<MonthlyBudget>("SELECT * FROM MonthlyBudgets WHERE AccountID = @AccountID", new { accountID }))
               .Select(b => new SelectListItem { Value = b.ID.ToString(), Text = b.StartDate.ToString("dd/MM/yyyy") + " - " + b.EndDate.ToString("dd/MM/yyyy") });
    }
}
