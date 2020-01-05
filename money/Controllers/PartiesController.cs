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
    public class PartiesController : MoneyControllerBase
    {
        public PartiesController(
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
                ? "Deleted = 1 OR Deleted = 0"
                : "Deleted = 0";

            var sql = $@"
SELECT
    p.ID,
    a.ID AS AccountID,
    a.Name AS Account,
    p.Name,
    p.Deleted
FROM
    Parties p
INNER JOIN
    Accounts a ON a.ID = p.AccountID
WHERE
    {whereClause}
ORDER BY
    a.DisplayOrder,
    p.Name";

            var parties = Db.Query(conn => conn.Query<ListPartiesPartyViewModel>(sql));

            return View(new ListPartiesViewModel {
                Parties = parties.GroupBy(p => (p.AccountID, p.Account))
            });
        }

        public IActionResult Create(int? id = null) =>
            View(new CreatePartyViewModel {
                Accounts = AccountsSelectListItems(),
                AccountID = id
            });

        [HttpPost]
        public IActionResult Create(CreatePartyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();

                return View(model);
            }

            Db.InsertOrUpdate(new Party(
                accountID: model.AccountID.Value,
                name: model.Name
            ));

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            var dto = Db.Get<Party>(id);

            return View(new UpdatePartyViewModel {
                ID = dto.ID,
                Name = dto.Name
            });
        }

        [HttpPost]
        public IActionResult Update(UpdatePartyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = Db.Get<Party>(model.ID);

            var updated = dto.WithUpdates(name: model.Name);

            Db.InsertOrUpdate(updated);

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var dto = Db.Get<Party>(id);

            Db.Delete(dto);

            UnitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<SelectListItem> AccountsSelectListItems() =>
            Db.Query(conn => conn.Query<Account>("SELECT * FROM Accounts"))
               .Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.Name });
    }
}
