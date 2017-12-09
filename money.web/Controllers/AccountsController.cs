using System.Web.Mvc;
using Dapper;
using money.web.Abstract;
using money.web.Models;
using money.web.Models.Entities;

namespace money.web.Controllers
{
    public class AccountsController : ControllerBase
    {
        public AccountsController(IUnitOfWork unitOfWork, IQueryHelper db, IRequestContext context) : base(unitOfWork, db, context) { }

        public ActionResult Index() => View(new ListAccountsViewModel {
            Accounts = _db.Query(conn => conn.Query<Account>("SELECT * FROM Accounts"))
        });

        public ActionResult Create() => View();

        [HttpPost]
        public ActionResult Create(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var account = new Account(
                userID: _userID,
                name: model.Name,
                isMainAccount: model.IsMainAccount,
                isIncludedInNetWorth: model.IncludeInNetWorth,
                displayOrder: model.DisplayOrder,
                startingBalance: model.StartingBalance
            );

            _db.InsertOrUpdate(account);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Update(int id)
        {
            var dto = _db.Get<Account>(id);

            return View(new UpdateAccountViewModel {
                ID = dto.ID,
                Name = dto.Name,
                IncludeInNetWorth = dto.IsIncludedInNetWorth,
                StartingBalance = dto.StartingBalance
            });
        }

        [HttpPost]
        public ActionResult Update(UpdateAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _db.Get<Account>(model.ID);

            var updated = dto.WithUpdates(
                name: model.Name,
                isIncludedInNetWorth: model.IncludeInNetWorth,
                startingBalance: model.StartingBalance
            );

            _db.InsertOrUpdate(updated);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var dto = _db.Get<Account>(id);

            _db.Delete(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
