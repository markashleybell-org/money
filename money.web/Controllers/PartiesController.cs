﻿using Dapper;
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
    public class PartiesController : ControllerBase
    {
        public PartiesController(IUnitOfWork unitOfWork, IQueryHelper db, IRequestContext context) : base(unitOfWork, db, context) { }

        public ActionResult Index()
        {
            return View(new ListPartiesViewModel {
                Parties = _db.Query(conn => conn.Query<PartyDTO>("SELECT * FROM Parties"))
            });
        }

        public ActionResult Create()
        {
            return View(new CreatePartyViewModel {
                Accounts = AccountsSelectListItems()
            });
        }

        [HttpPost]
        public ActionResult Create(CreatePartyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Accounts = AccountsSelectListItems();
                return View(model);
            }

            _db.InsertOrUpdate(new PartyDTO {
                AccountID = model.AccountID,
                Name = model.Name
            });

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Update(int id)
        {
            var dto = _db.Get<PartyDTO>(id);

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

            var dto = _db.Get<PartyDTO>(model.ID);

            dto.AccountID = model.AccountID;
            dto.Name = model.Name;

            _db.InsertOrUpdate(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var dto = _db.Get<PartyDTO>(id);

            _db.Delete(dto);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<SelectListItem> AccountsSelectListItems()
        {
            return _db.Query(conn => conn.Query<AccountDTO>("SELECT * FROM Accounts"))
                .Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.Name });
        }
    }
}