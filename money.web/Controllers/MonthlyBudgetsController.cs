using Dapper;
using Dapper.Contrib.Extensions;
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

        public ActionResult Index(int id)
        {
            return View(new ListMonthlyBudgetsViewModel {
                AccountID = id,
                MonthlyBudgets = _db.Query(conn => conn.Query<MonthlyBudgetDTO>("SELECT * FROM MonthlyBudgets WHERE AccountID = @ID", new { id }))
            });
        }

        public ActionResult Create(int id)
        {
            return View(new CreateMonthlyBudgetViewModel {
                AccountID = id,
                StartDate = DateTime.Now.FirstDayOfMonth(),
                EndDate = DateTime.Now.LastDayOfMonth(),
                Categories = Categories(accountID: id)
            });
        }

        [HttpPost]
        public ActionResult Create(CreateMonthlyBudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = Categories(accountID: model.AccountID);
                return View(model);
            }

            var monthlyBudgetID = _db.InsertOrUpdate(new MonthlyBudgetDTO {
                AccountID = model.AccountID,
                StartDate = model.StartDate.ZeroTime(),
                EndDate = model.EndDate.SetTime(23, 59, 59)
            });

            var categories = model.Categories.Where(c => c.Amount != 0).Select(c => new Category_MonthlyBudgetDTO {
                MonthlyBudgetID = monthlyBudgetID,
                CategoryID = c.CategoryID,
                Amount = c.Amount
            });

            foreach (var category in categories)
                _db.Execute((conn, tran) => conn.Insert(category, tran));

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index), new { id = model.AccountID });
        }

        public ActionResult Update(int id)
        {
            var dto = _db.Get<MonthlyBudgetDTO>(id);

            return View(new UpdateMonthlyBudgetViewModel {
                ID = dto.ID,
                AccountID = dto.AccountID,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Categories = Categories(accountID: dto.AccountID, monthlyBudgetID: dto.ID)
            });
        }

        [HttpPost]
        public ActionResult Update(UpdateMonthlyBudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = Categories(accountID: model.AccountID, monthlyBudgetID: model.ID);
                return View(model);
            }

            var dto = _db.Get<MonthlyBudgetDTO>(model.ID);

            dto.AccountID = model.AccountID;
            dto.StartDate = model.StartDate.ZeroTime();
            dto.EndDate = model.EndDate.SetTime(23, 59, 59);

            _db.InsertOrUpdate(dto);

            _db.Execute((conn, tran) => conn.Execute("DELETE FROM Categories_MonthlyBudgets WHERE MonthlyBudgetID = @ID", new { model.ID }, tran));

            var categories = model.Categories.Where(c => c.Amount != 0).Select(c => new Category_MonthlyBudgetDTO {
                MonthlyBudgetID = model.ID,
                CategoryID = c.CategoryID,
                Amount = c.Amount
            });

            foreach (var category in categories)
                _db.Execute((conn, tran) => conn.Insert(category, tran));

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index), new { id = model.AccountID });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var dto = _db.Get<MonthlyBudgetDTO>(id);

            _db.Delete(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index), new { id = dto.AccountID });
        }

        private IEnumerable<MonthlyBudgetCategoryViewModel> Categories(int accountID, int monthlyBudgetID = 0)
        {
            var sql = @"SELECT 
                            c.ID AS CategoryID,
                            c.Name,
                            b.Amount
                        FROM
                            Categories c
                        LEFT JOIN
                            Categories_MonthlyBudgets b
                        ON
                            b.CategoryID = c.ID
                        AND
                            b.MonthlyBudgetID = @MonthlyBudgetID
                        WHERE
                            c.AccountID = @AccountID";

            var parameters = new { accountID, monthlyBudgetID };

            return _db.Query(conn => conn.Query<MonthlyBudgetCategoryViewModel>(sql, parameters));
        }
    }
}