using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using money.Controllers;
using money.Entities;
using money.Models;
using money.Support;

namespace money.web.Controllers
{
    public class MonthlyBudgetsController : MoneyControllerBase
    {
        public MonthlyBudgetsController(
            IOptionsMonitor<Settings> optionsMonitor,
            IUnitOfWork unitOfWork,
            IQueryHelper db,
            IRequestContext ctx)
            : base(
                  optionsMonitor,
                  unitOfWork,
                  db,
                  ctx)
        {
        }

        public IActionResult Index(int id)
        {
            var sql = @"SELECT 
                            * 
                        FROM
                            MonthlyBudgets
                        WHERE
                            AccountID = @ID 
                        ORDER BY 
                            StartDate DESC";

            return View(new ListMonthlyBudgetsViewModel
            {
                AccountID = id,
                MonthlyBudgets = _db.Query(conn => conn.Query<MonthlyBudget>(sql, new { id }))
            });
        }

        public IActionResult Create(int id) => View(new CreateMonthlyBudgetViewModel
        {
            AccountID = id,
            StartDate = DateTime.Now.FirstDayOfMonth(),
            EndDate = DateTime.Now.LastDayOfMonth(),
            Categories = Categories(accountID: id)
        });

        [HttpPost]
        public IActionResult Create(CreateMonthlyBudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = Categories(accountID: model.AccountID);

                return View(model);
            }

            var monthlyBudgetID = _db.InsertOrUpdate(new MonthlyBudget(
                accountID: model.AccountID,
                startDate: model.StartDate.WithZeroedTime(),
                endDate: model.EndDate.SetTime(23, 59, 59)
            ));

            var categories = model.Categories.Where(c => c.Amount != 0).Select(c => new Category_MonthlyBudget(
                monthlyBudgetId: monthlyBudgetID,
                categoryId: c.CategoryID,
                amount: c.Amount < 0 ? c.Amount : -c.Amount
            ));

            foreach (var category in categories)
            {
                _db.Execute((conn, tran) => conn.Insert(category, tran));
            }

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index), new { id = model.AccountID });
        }

        public IActionResult Update(int id)
        {
            var dto = _db.Get<MonthlyBudget>(id);

            return View(new UpdateMonthlyBudgetViewModel
            {
                ID = dto.ID,
                AccountID = dto.AccountID,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Categories = Categories(accountID: dto.AccountID, monthlyBudgetID: dto.ID)
            });
        }

        [HttpPost]
        public IActionResult Update(UpdateMonthlyBudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = Categories(accountID: model.AccountID, monthlyBudgetID: model.ID);

                return View(model);
            }

            var dto = _db.Get<MonthlyBudget>(model.ID);

            var updated = dto.WithUpdates(
                startDate: model.StartDate.WithZeroedTime(),
                endDate: model.EndDate.SetTime(23, 59, 59)
            );

            _db.InsertOrUpdate(updated);

            _db.Execute((conn, tran) => conn.Execute("DELETE FROM Categories_MonthlyBudgets WHERE MonthlyBudgetID = @ID", new { model.ID }, tran));

            var categories = model.Categories.Where(c => c.Amount != 0).Select(c => new Category_MonthlyBudget(
                monthlyBudgetId: model.ID,
                categoryId: c.CategoryID,
                amount: c.Amount < 0 ? c.Amount : -c.Amount
            ));

            foreach (var category in categories)
            {
                _db.Execute((conn, tran) => conn.Insert(category, tran));
            }

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index), new { id = model.AccountID });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var dto = _db.Get<MonthlyBudget>(id);

            var sql = "DELETE FROM Categories_MonthlyBudgets WHERE MonthlyBudgetID = @ID";

            _db.Execute((conn, tran) => conn.Execute(sql, new { id }, tran));

            _db.Delete(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index), new { id = dto.AccountID });
        }

        public IActionResult Copy(int id)
        {
            var dto = _db.Get<MonthlyBudget>(id);

            // Start the day after the old budget finished
            var startDate = dto.EndDate.AddDays(1);

            // End on the last day of the same month
            var endDate = startDate.LastDayOfMonth();

            // If the budget started just before the end of a month,
            // skip the end date to the end of the *next* month
            if ((endDate - startDate).Days < 28)
            {
                endDate = endDate.LastDayOfNextMonth();
            }

            var model = new CreateMonthlyBudgetViewModel
            {
                AccountID = dto.AccountID,
                StartDate = startDate,
                EndDate = endDate,
                Categories = Categories(accountID: dto.AccountID, monthlyBudgetID: dto.ID)
            };

            return View(nameof(Create), model);
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
