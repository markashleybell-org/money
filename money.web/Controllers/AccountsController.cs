using Dapper;
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
    public class AccountsController : ControllerBase
    {
        public AccountsController(IUnitOfWork unitOfWork, IQueryHelper db, IRequestContext context) : base(unitOfWork, db, context) { }

        public ActionResult Index()
        {
            return View(new ListAccountsViewModel {
                Accounts = _db.Query(conn => conn.Query<AccountDTO>("SELECT * FROM Accounts"))
            });
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _db.InsertOrUpdate(new AccountDTO {
                UserID = _userID,
                Name = model.Name,
                Type = (int)model.Type,
                IsMainAccount = model.IsMainAccount,
                IsIncludedInNetWorth = model.IncludeInNetWorth,
                DisplayOrder = model.DisplayOrder,
                StartingBalance = model.StartingBalance
            });

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Update(int id)
        {
            var dto = _db.Get<AccountDTO>(id);

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

            var dto = _db.Get<AccountDTO>(model.ID);

            dto.Name = model.Name;
            dto.IsIncludedInNetWorth = model.IncludeInNetWorth;
            dto.StartingBalance = model.StartingBalance;

            _db.InsertOrUpdate(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var dto = _db.Get<AccountDTO>(id);

            _db.Delete(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}