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
                Accounts = AccountsSelectListItems()
            });
        }

        [HttpPost]
        public ActionResult Create(CreateEntryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();
                return View(model);
            }

            _db.InsertOrUpdate(new EntryDTO {
                AccountID = model.AccountID,
                MonthlyBudgetID = model.MonthlyBudgetID,
                CategoryID = model.CategoryID,
                PartyID = model.PartyID,
                Date = model.Date,
                Amount = model.Amount,
                Note = model.Note
            });

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
    }
}