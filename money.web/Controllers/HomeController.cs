using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using money.web.Abstract;
using money.web.Models;
using money.web.Models.Entities;
using money.web.Support;

namespace money.web.Controllers
{
    public class HomeController : ControllerBase
    {
        public HomeController(IUnitOfWork unitOfWork, IQueryHelper db, IRequestContext context) : base(unitOfWork, db, context) { }

        public ActionResult Index()
        {
            var model = _db.Query(conn => {
                using (var reader = conn.QueryMultipleSP("Dashboard", new { UserID = _userID }))
                {
                    var accounts = reader.Read<AccountViewModel>();
                    var categories = reader.Read<CategoryViewModel>();

                    foreach (var account in accounts)
                        account.Categories = categories.Where(c => c.AccountID == account.ID);

                    return new IndexViewModel {
                        Accounts = accounts
                    };
                }
            });

            return View(model);
        }

        public ActionResult AddEntry(int id) => View(new AddEntryViewModel {
            AccountID = id,
            MonthlyBudgetID = GetLatestMonthlyBudget(id),
            Accounts = AccountsSelectListItems(),
            MonthlyBudgets = MonthlyBudgetsSelectListItems(id),
            Categories = CategoriesSelectListItems(id),
            Parties = PartiesSelectListItems(id)
        });

        [HttpPost]
        public ActionResult AddEntry(AddEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();
                model.MonthlyBudgets = MonthlyBudgetsSelectListItems(model.AccountID);
                model.Categories = CategoriesSelectListItems(model.AccountID);
                model.Parties = PartiesSelectListItems(model.AccountID);
                return View(model);
            }

            var amount = Math.Abs(model.Amount);

            if (model.TransferAccountID.HasValue)
            {
                var destinationAccountID = model.TransferAccountID.Value;

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
            }
            else
            {
                if (model.Type == EntryType.Debit)
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
            }

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        private int? GetLatestMonthlyBudget(int accountID)
        {
            var sql = "SELECT TOP 1 ID FROM MonthlyBudgets WHERE AccountID = @AccountID AND EndDate <= GETDATE() ORDER BY EndDate, ID";
            return _db.Query(conn => conn.QuerySingleOrDefault<int?>(sql, new { accountID }));
        }

        private IEnumerable<SelectListItem> AccountsSelectListItems() =>
            _db.Query(conn => conn.Query<Account>("SELECT * FROM Accounts"))
               .Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.Name });

        private IEnumerable<SelectListItem> CategoriesSelectListItems(int accountID) =>
            _db.Query(conn => conn.Query<Category>("SELECT * FROM Categories WHERE AccountID = @AccountID", new { accountID }))
               .Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name });

        private IEnumerable<SelectListItem> PartiesSelectListItems(int accountID) =>
            _db.Query(conn => conn.Query<Party>("SELECT * FROM Parties WHERE AccountID = @AccountID", new { accountID }))
               .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = p.Name });

        private IEnumerable<SelectListItem> MonthlyBudgetsSelectListItems(int accountID) =>
            _db.Query(conn => conn.Query<MonthlyBudget>("SELECT * FROM MonthlyBudgets WHERE AccountID = @AccountID", new { accountID }))
               .Select(b => new SelectListItem { Value = b.ID.ToString(), Text = b.StartDate.ToString("dd/MM/yyyy") + " - " + b.EndDate.ToString("dd/MM/yyyy") });
    }
}
