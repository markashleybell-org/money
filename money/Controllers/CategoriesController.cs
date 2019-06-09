using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using money.Controllers;
using money.Entities;
using money.Models;
using money.Support;

namespace money.web.Controllers
{
    public class CategoriesController : MoneyControllerBase
    {
        public CategoriesController(
            IOptionsMonitor<Settings> optionsMonitor,
            IUnitOfWork unitOfWork,
            IQueryHelper db)
            : base(
                  optionsMonitor,
                  unitOfWork,
                  db)
        {
        }

        public IActionResult Index()
        {
            var sql = @"SELECT 
                            c.ID, 
                            a.Name AS Account, 
                            c.Name 
                        FROM 
                            Categories c 
                        INNER JOIN 
                            Accounts a ON a.ID = c.AccountID
                        ORDER BY
                            a.DisplayOrder,
                            c.DisplayOrder";

            var categories = _db.Query(conn => conn.Query<ListCategoriesCategoryViewModel>(sql));

            return View(new ListCategoriesViewModel
            {
                Categories = categories.GroupBy(c => c.Account)
            });
        }

        public IActionResult Create() =>
            View(new CreateCategoryViewModel
            {
                Accounts = AccountsSelectListItems()
            });

        [HttpPost]
        public IActionResult Create(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();

                return View(model);
            }

            var category = new Category(
                accountID: model.AccountID,
                name: model.Name
            );

            _db.InsertOrUpdate(category);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            var dto = _db.Get<Category>(id);

            return View(new UpdateCategoryViewModel
            {
                ID = dto.ID,
                Name = dto.Name
            });
        }

        [HttpPost]
        public IActionResult Update(UpdateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = _db.Get<Category>(model.ID);

            var updated = dto.WithUpdates(name: model.Name);

            _db.InsertOrUpdate(updated);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var dto = _db.Get<Category>(id);

            _db.Delete(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<SelectListItem> AccountsSelectListItems() =>
            _db.Query(conn => conn.Query<Account>("SELECT * FROM Accounts"))
               .Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.Name });
    }
}
