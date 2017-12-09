using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using money.web.Abstract;
using money.web.Models;
using money.web.Models.Entities;

namespace money.web.Controllers
{
    public class PartiesController : ControllerBase
    {
        public PartiesController(IUnitOfWork unitOfWork, IQueryHelper db, IRequestContext context) : base(unitOfWork, db, context) { }

        public ActionResult Index() => View(new ListPartiesViewModel {
            Parties = _db.Query(conn => conn.Query<Party>("SELECT * FROM Parties"))
        });

        public ActionResult Create() => View(new CreatePartyViewModel {
            Accounts = AccountsSelectListItems()
        });

        [HttpPost]
        public ActionResult Create(CreatePartyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();
                return View(model);
            }

            _db.InsertOrUpdate(new Party(
                accountID: model.AccountID,
                name: model.Name
            ));

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Update(int id)
        {
            var dto = _db.Get<Party>(id);

            return View(new UpdatePartyViewModel {
                ID = dto.ID,
                AccountID = dto.AccountID,
                Name = dto.Name,
                Accounts = AccountsSelectListItems()
            });
        }

        [HttpPost]
        public ActionResult Update(UpdatePartyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();
                return View(model);
            }

            var dto = _db.Get<Party>(model.ID);

            var updated = dto.WithUpdates(name: model.Name);

            _db.InsertOrUpdate(updated);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var dto = _db.Get<Party>(id);

            _db.Delete(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<SelectListItem> AccountsSelectListItems() =>
            _db.Query(conn => conn.Query<Account>("SELECT * FROM Accounts"))
               .Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.Name });
    }
}
