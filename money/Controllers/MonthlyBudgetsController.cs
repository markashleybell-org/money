using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Money.Entities;
using Money.Models;
using Money.Support;

namespace Money.Controllers
{
    public class MonthlyBudgetsController : MoneyControllerBase
    {
        public MonthlyBudgetsController(
            IOptionsMonitor<Settings> optionsMonitor,
            IDateTimeService dateTimeService,
            IUnitOfWork unitOfWork,
            IQueryHelper db)
            : base(
                  optionsMonitor,
                  dateTimeService,
                  unitOfWork,
                  db)
        {
        }

        public IActionResult Index(int id)
        {
            const string sql = @"
SELECT
    *
FROM
    MonthlyBudgets
WHERE
    AccountID = @ID
ORDER BY
    StartDate DESC";

            var model = new ListMonthlyBudgetsViewModel {
                AccountID = id,
                AccountName = AccountName(accountID: id),
                MonthlyBudgets = Db.Query(conn => conn.Query<MonthlyBudget>(sql, new { id }))
            };

            return View(model);
        }

        public IActionResult Create(int id)
        {
            var model = new CreateMonthlyBudgetViewModel {
                AccountID = id,
                AccountName = AccountName(accountID: id),
                StartDate = DateTime.Now.FirstDayOfMonth(),
                EndDate = DateTime.Now.LastDayOfMonth(),
                Categories = Categories(accountID: id)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateMonthlyBudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AccountName = AccountName(accountID: model.AccountID);
                model.Categories = Categories(accountID: model.AccountID);

                return View(model);
            }

            var monthlyBudgetID = Db.InsertOrUpdate(new MonthlyBudget(
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
                Db.Execute((conn, tran) => conn.Insert(category, tran));
            }

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index), new { id = model.AccountID });
        }

        public IActionResult Update(int id)
        {
            var dto = Db.Get<MonthlyBudget>(id);

            var model = new UpdateMonthlyBudgetViewModel {
                ID = dto.ID,
                AccountID = dto.AccountID,
                AccountName = AccountName(accountID: dto.AccountID),
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Categories = Categories(accountID: dto.AccountID, monthlyBudgetID: dto.ID)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(UpdateMonthlyBudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AccountName = AccountName(accountID: model.AccountID);
                model.Categories = Categories(accountID: model.AccountID, monthlyBudgetID: model.ID);

                return View(model);
            }

            var dto = Db.Get<MonthlyBudget>(model.ID);

            var updated = dto.WithUpdates(
                startDate: model.StartDate.WithZeroedTime(),
                endDate: model.EndDate.SetTime(23, 59, 59)
            );

            Db.InsertOrUpdate(updated);

            Db.Execute((conn, tran) => conn.Execute("DELETE FROM Categories_MonthlyBudgets WHERE MonthlyBudgetID = @ID", new { model.ID }, tran));

            var categories = model.Categories.Where(c => c.Amount != 0).Select(c => new Category_MonthlyBudget(
                monthlyBudgetId: model.ID,
                categoryId: c.CategoryID,
                amount: c.Amount < 0 ? c.Amount : -c.Amount
            ));

            foreach (var category in categories)
            {
                Db.Execute((conn, tran) => conn.Insert(category, tran));
            }

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index), new { id = model.AccountID });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var dto = Db.Get<MonthlyBudget>(id);

            const string sql = @"
DELETE FROM
    Categories_MonthlyBudgets
WHERE
    MonthlyBudgetID = @ID";

            Db.Execute((conn, tran) => conn.Execute(sql, new { id }, tran));

            Db.Delete(dto);

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index), new { id = dto.AccountID });
        }

        public IActionResult Copy(int id)
        {
            var dto = Db.Get<MonthlyBudget>(id);

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

            var model = new CreateMonthlyBudgetViewModel {
                AccountID = dto.AccountID,
                AccountName = AccountName(dto.AccountID),
                StartDate = startDate,
                EndDate = endDate,
                Categories = Categories(accountID: dto.AccountID, monthlyBudgetID: dto.ID)
            };

            return View(nameof(Create), model);
        }

        private string AccountName(int accountID)
        {
            const string sql = @"
SELECT
    Name
FROM
    Accounts a
WHERE
    a.ID = @AccountID";

            return Db.Query(conn => conn.QuerySingleOrDefault<string>(sql, new { accountID }));
        }

        private IEnumerable<MonthlyBudgetCategoryViewModel> Categories(int accountID, int monthlyBudgetID = 0)
        {
            const string sql = @"
SELECT
    c.ID AS CategoryID,
    c.Name,
    b.Amount,
    c.Deleted AS CategoryDeleted
FROM
    Categories c
LEFT JOIN
    Categories_MonthlyBudgets b
ON
    b.CategoryID = c.ID
AND
    b.MonthlyBudgetID = @MonthlyBudgetID
WHERE
    c.AccountID = @AccountID
ORDER BY
    c.DisplayOrder";

            var parameters = new { accountID, monthlyBudgetID };

            return Db.Query(conn => conn.Query<MonthlyBudgetCategoryViewModel>(sql, parameters));
        }
    }
}
