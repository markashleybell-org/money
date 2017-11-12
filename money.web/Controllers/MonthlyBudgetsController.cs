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
    public class MonthlyBudgetsController : ControllerBase
    {
        public MonthlyBudgetsController(IUnitOfWork unitOfWork, IQueryHelper db, IRequestContext context) : base(unitOfWork, db, context) { }

        public ActionResult Index()
        {
            return View(new ListMonthlyBudgetsViewModel {
                MonthlyBudgets = _db.Query(conn => conn.Query<MonthlyBudgetDTO>("SELECT * FROM MonthlyBudgets"))
            });
        }

        public ActionResult Create()
        {
            return View(new CreateMonthlyBudgetViewModel {
                Accounts = AccountsSelectListItems(),
                StartDate = DateTime.Now.FirstDayOfMonth(),
                EndDate = DateTime.Now.LastDayOfMonth()
            });
        }

        [HttpPost]
        public ActionResult Create(CreateMonthlyBudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();
                return View(model);
            }

            _db.InsertOrUpdate(new MonthlyBudgetDTO {
                AccountID = model.AccountID,
                StartDate = model.StartDate.ZeroTime(),
                EndDate = model.EndDate.SetTime(23, 59, 59)
            });

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Update(int id)
        {
            var dto = _db.Get<MonthlyBudgetDTO>(id);

            return View(new UpdateMonthlyBudgetViewModel {
                ID = dto.ID,
                AccountID = dto.AccountID,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Accounts = AccountsSelectListItems()
            });
        }

        [HttpPost]
        public ActionResult Update(UpdateMonthlyBudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();
                return View(model);
            }

            var dto = _db.Get<MonthlyBudgetDTO>(model.ID);

            dto.AccountID = model.AccountID;
            dto.StartDate = model.StartDate.ZeroTime();
            dto.EndDate = model.EndDate.SetTime(23, 59, 59);

            _db.InsertOrUpdate(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var dto = _db.Get<MonthlyBudgetDTO>(id);

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