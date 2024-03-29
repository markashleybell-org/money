using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Money.Entities;
using Money.Models;
using Money.Support;

namespace Money.Controllers
{
    public class AccountsController : MoneyControllerBase
    {
        public AccountsController(
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

        public IActionResult Index()
        {
            var model = new ListAccountsViewModel {
                Accounts = Db.Query(conn => conn.Query<Account>("SELECT * FROM Accounts"))
            };

            return View(model);
        }

        public IActionResult Create()
        {
            var model = new CreateAccountViewModel();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var account = new Account(
                userID: UserID,
                name: model.Name,
                type: model.Type.Value,
                startingBalance: model.StartingBalance.Value,
                isMainAccount: model.IsMainAccount,
                isIncludedInNetWorth: model.IncludeInNetWorth,
                isDormant: false,
                displayOrder: model.DisplayOrder.Value,
                numberLast4Digits: model.NumberLast4Digits
            );

            Db.InsertOrUpdate(account);

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            var dto = Db.Get<Account>(id);

            var model = new UpdateAccountViewModel {
                ID = dto.ID,
                Name = dto.Name,
                Type = dto.Type,
                StartingBalance = dto.StartingBalance,
                IncludeInNetWorth = dto.IsIncludedInNetWorth,
                DisplayOrder = dto.DisplayOrder,
                IsDormant = dto.IsDormant,
                NumberLast4Digits = dto.NumberLast4Digits
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(UpdateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = Db.Get<Account>(model.ID);

            var updated = dto.WithUpdates(
                name: model.Name,
                type: model.Type,
                startingBalance: model.StartingBalance.Value,
                displayOrder: model.DisplayOrder.Value,
                isIncludedInNetWorth: model.IncludeInNetWorth,
                isDormant: model.IsDormant,
                numberLast4Digits: model.NumberLast4Digits);

            Db.InsertOrUpdate(updated);

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateDisplayOrder([FromBody] DisplayOrderUpdateModel model)
        {
            Db.UpdateDisplayOrder<Account>(model.ItemOrder);

            UnitOfWork.CommitChanges();

            return Ok();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var dto = Db.Get<Account>(id);

            Db.Delete(dto);

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
