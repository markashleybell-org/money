using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Money.Entities;
using Money.Models;
using Money.Support;

namespace Money.Controllers
{
    public class CategoriesController : MoneyControllerBase
    {
        public CategoriesController(
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

        public IActionResult Index(bool? showDeleted = false)
        {
            var whereClause = showDeleted == true
                ? "c.Deleted = 1 OR c.Deleted = 0"
                : "c.Deleted = 0";

            var sql = @$"
SELECT
    c.ID,
    a.ID AS AccountID,
    a.Name AS Account,
    c.Name,
    c.Deleted
FROM
    Categories c
INNER JOIN
    Accounts a ON a.ID = c.AccountID
WHERE
    {whereClause}
ORDER BY
    a.DisplayOrder,
    c.DisplayOrder";

            var categories = Db.Query(conn => conn.Query<ListCategoriesCategoryViewModel>(sql));

            var model = new ListCategoriesViewModel {
                Categories = categories.GroupBy(c => (c.AccountID, c.Account))
            };

            return View(model);
        }

        public IActionResult Create(int? id = null)
        {
            var model = new CreateCategoryViewModel {
                Accounts = AccountsSelectListItems(),
                AccountID = id
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();

                return View(model);
            }

            var category = new Category(
                accountID: model.AccountID.Value,
                name: model.Name
            );

            Db.InsertOrUpdate(category);

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            var dto = Db.Get<Category>(id);

            var model = new UpdateCategoryViewModel {
                ID = dto.ID,
                Name = dto.Name,
                IsDeleted = dto.Deleted
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(UpdateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = Db.Get<Category>(model.ID);

            var updated = dto.WithUpdates(name: model.Name);

            Db.InsertOrUpdate(updated);

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateDisplayOrder([FromBody] DisplayOrderUpdateModel model)
        {
            Db.UpdateDisplayOrder<Category>(model.ItemOrder);

            UnitOfWork.CommitChanges();

            return Ok();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var dto = Db.Get<Category>(id);

            Db.Delete(dto);

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Undelete(int id)
        {
            var dto = Db.Get<Category>(id);

            Db.Undelete(dto);

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<SelectListItem> AccountsSelectListItems() =>
            Db.Query(conn => conn.Query<Account>("SELECT * FROM Accounts"))
               .Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.Name });
    }
}
