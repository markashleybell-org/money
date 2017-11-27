using Dapper;
using money.common;
using money.web.Abstract;
using money.web.Models;
using money.web.Models.DTO;
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
                Entries = _db.Query(conn => conn.Query<EntryDTO>("SELECT * FROM Entries"))
            });
        }

        public ActionResult Create()
        {
            return View(new CreateEntryViewModel {
                Accounts = AccountsSelectListItems(),
                MonthlyBudgets = MonthlyBudgetsSelectListItems(),
                Categories = CategoriesSelectListItems(),
                Parties = PartiesSelectListItems()
            });
        }

        [HttpPost]
        public ActionResult Create(CreateEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();
                model.MonthlyBudgets = MonthlyBudgetsSelectListItems();
                model.Categories = CategoriesSelectListItems();
                model.Parties = PartiesSelectListItems();
                return View(model);
            }

            var amount = Math.Abs(model.Amount);

            if (model.TransferAccountID.HasValue)
            {
                var destinationAccountID = model.TransferAccountID.Value;

                var parameters = new { ids = new[] { model.AccountID, destinationAccountID } };

                var accounts = _db.Query(conn => conn.Query<AccountDTO>("SELECT * FROM Accounts WHERE ID IN @ids", parameters));
                var sourceAccountName = accounts.Single(a => a.ID == model.AccountID).Name;
                var destinationAccountName = accounts.Single(a => a.ID == destinationAccountID).Name;

                var guid = Guid.NewGuid();

                // For transfers, we set up separate entries for source and destination accounts
                // Note that we ignore the credit/debit selection here (a transfer is always a debit)

                _db.InsertOrUpdate(new EntryDTO {
                    AccountID = model.AccountID,
                    MonthlyBudgetID = model.MonthlyBudgetID,
                    CategoryID = model.CategoryID,
                    Date = model.Date,
                    Amount = -amount,
                    Note = $"Transfer to {destinationAccountName}",
                    TransferGUID = guid
                });

                // TODO: Get latest monthly budget ID for destination 

                _db.InsertOrUpdate(new EntryDTO {
                    AccountID = destinationAccountID,
                    Date = model.Date,
                    Amount = amount,
                    Note = $"Transfer from {sourceAccountName}",
                    TransferGUID = guid
                });
            }
            else
            {
                if (model.Type == EntryType.Debit)
                    amount = -amount;
                
                _db.InsertOrUpdate(new EntryDTO {
                    AccountID = model.AccountID,
                    MonthlyBudgetID = model.MonthlyBudgetID,
                    CategoryID = model.CategoryID,
                    PartyID = model.PartyID,
                    Date = model.Date,
                    Amount = amount,
                    Note = model.Note
                });
            }

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Update(int id)
        {
            var dto = _db.Get<EntryDTO>(id);

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

            var dto = _db.Get<EntryDTO>(model.ID);

            dto.AccountID = model.AccountID;
            dto.MonthlyBudgetID = model.MonthlyBudgetID;
            dto.CategoryID = model.CategoryID;
            dto.PartyID = model.PartyID;
            dto.Date = model.Date;
            dto.Amount = model.Amount;
            dto.Note = model.Note;

            _db.InsertOrUpdate(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var dto = _db.Get<EntryDTO>(id);

            _db.Delete(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<SelectListItem> AccountsSelectListItems()
        {
            return _db.Query(conn => conn.Query<AccountDTO>("SELECT * FROM Accounts"))
                .Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.Name });
        }

        private IEnumerable<SelectListItem> CategoriesSelectListItems()
        {
            return _db.Query(conn => conn.Query<AccountDTO>("SELECT * FROM Categories"))
                .Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name });
        }

        private IEnumerable<SelectListItem> PartiesSelectListItems()
        {
            return _db.Query(conn => conn.Query<AccountDTO>("SELECT * FROM Parties"))
                .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = p.Name });
        }

        private IEnumerable<SelectListItem> MonthlyBudgetsSelectListItems()
        {
            return _db.Query(conn => conn.Query<MonthlyBudgetDTO>("SELECT * FROM MonthlyBudgets"))
                .Select(b => new SelectListItem { Value = b.ID.ToString(), Text = b.StartDate.ToString("dd/MM/yyyy") + " - " + b.EndDate.ToString("dd/MM/yyyy") });
        }
    }
}