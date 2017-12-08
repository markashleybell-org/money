using Dapper;
using money.common;
using money.web.Abstract;
using money.web.Models;
using money.web.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace money.web.Controllers
{
    public class EntriesController : ControllerBase
    {
        public EntriesController(IUnitOfWork unitOfWork, IQueryHelper db, IRequestContext context) : base(unitOfWork, db, context) { }

        public ActionResult Index()
        {
            return View(new ListEntriesViewModel {
                Entries = _db.Query(conn => conn.Query<Entry>("SELECT * FROM Entries"))
            });
        }

        public ActionResult Create(int? id)
        {
            var accountID = id ?? default;

            return View(new CreateEntryViewModel {
                AccountID = accountID,
                Accounts = AccountsSelectListItems(),
                MonthlyBudgets = MonthlyBudgetsSelectListItems(accountID),
                Categories = CategoriesSelectListItems(accountID),
                Parties = PartiesSelectListItems(accountID)
            });
        }

        [HttpPost]
        public ActionResult Create(CreateEntryViewModel model)
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
                    monthlyBudgetID: null, // TODO: Get latest monthly budget ID for destination 
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
                Accounts = AccountsSelectListItems()
            });
        }

        [HttpPost]
        public ActionResult Update(UpdateEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();
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

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var dto = _db.Get<Entry>(id);

            _db.Delete(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<SelectListItem> AccountsSelectListItems()
        {
            return _db.Query(conn => conn.Query<Account>("SELECT * FROM Accounts"))
                .Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.Name });
        }

        private IEnumerable<SelectListItem> CategoriesSelectListItems(int accountID)
        {
            return _db.Query(conn => conn.Query<Category>("SELECT * FROM Categories WHERE AccountID = @AccountID", new { accountID }))
                .Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name });
        }

        private IEnumerable<SelectListItem> PartiesSelectListItems(int accountID)
        {
            return _db.Query(conn => conn.Query<Party>("SELECT * FROM Parties WHERE AccountID = @AccountID", new { accountID }))
                .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = p.Name });
        }

        private IEnumerable<SelectListItem> MonthlyBudgetsSelectListItems(int accountID)
        {
            return _db.Query(conn => conn.Query<MonthlyBudget>("SELECT * FROM MonthlyBudgets WHERE AccountID = @AccountID", new { accountID }))
                .Select(b => new SelectListItem { Value = b.ID.ToString(), Text = b.StartDate.ToString("dd/MM/yyyy") + " - " + b.EndDate.ToString("dd/MM/yyyy") });
        }
    }
}