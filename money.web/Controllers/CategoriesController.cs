using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using money.web.Abstract;
using money.web.Models;
using money.web.Models.Entities;

namespace money.web.Controllers
{
    public class CategoriesController : ControllerBase
    {
        public CategoriesController(IUnitOfWork unitOfWork, IQueryHelper db, IRequestContext context) : base(unitOfWork, db, context) { }

        public ActionResult Index() => View(new ListCategoriesViewModel {
            Categories = _db.Query(conn => conn.Query<Category>("SELECT * FROM Categories"))
        });

        public ActionResult Create() => View(new CreateCategoryViewModel {
            Accounts = AccountsSelectListItems()
        });

        [HttpPost]
        public ActionResult Create(CreateCategoryViewModel model)
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

        public ActionResult Update(int id)
        {
            var dto = _db.Get<Category>(id);

            return View(new UpdateCategoryViewModel {
                ID = dto.ID,
                AccountID = dto.AccountID,
                Name = dto.Name,
                Accounts = AccountsSelectListItems()
            });
        }

        [HttpPost]
        public ActionResult Update(UpdateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();
                return View(model);
            }

            var dto = _db.Get<Category>(model.ID);

            var updated = dto.WithUpdates(name: model.Name);

            _db.InsertOrUpdate(updated);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public ActionResult Delete(int id)
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
