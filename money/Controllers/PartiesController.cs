﻿using System.Collections.Generic;
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
    public class PartiesController : MoneyControllerBase
    {
        public PartiesController(
            IOptionsMonitor<Settings> optionsMonitor,
            IUnitOfWork unitOfWork,
            IQueryHelper db,
            IRequestContext ctx)
            : base(
                  optionsMonitor,
                  unitOfWork,
                  db,
                  ctx)
        {
        }

        public IActionResult Index()
        {
            var sql = @"SELECT 
                            p.ID, 
                            a.Name AS Account, 
                            p.Name 
                        FROM 
                            Parties p 
                        INNER JOIN 
                            Accounts a ON a.ID = p.AccountID
                        ORDER BY
                            a.DisplayOrder,
                            p.Name";

            var parties = _db.Query(conn => conn.Query<ListPartiesPartyViewModel>(sql));

            return View(new ListPartiesViewModel
            {
                Parties = parties.GroupBy(p => p.Account)
            });
        }

        public IActionResult Create() => View(new CreatePartyViewModel
        {
            Accounts = AccountsSelectListItems()
        });

        [HttpPost]
        public IActionResult Create(CreatePartyViewModel model)
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

        public IActionResult Update(int id)
        {
            var dto = _db.Get<Party>(id);

            return View(new UpdatePartyViewModel
            {
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

            var dto = _db.Get<Party>(model.ID);

            var updated = dto.WithUpdates(name: model.Name);

            _db.InsertOrUpdate(updated);

            _unitOfWork.CommitChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
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
