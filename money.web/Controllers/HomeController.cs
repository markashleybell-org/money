using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using money.web.Abstract;
using money.web.Models;
using money.web.Models.Entities;
using money.web.Support;
using static money.web.Support.Extensions;

namespace money.web.Controllers
{
    public class HomeController : ControllerBase
    {
        public HomeController(IUnitOfWork unitOfWork, IQueryHelper db, IRequestContext context) : base(unitOfWork, db, context) { }

        public ActionResult Index()
        {
            var model = _db.Query(conn => {
                using (var reader = conn.QueryMultipleSP("Dashboard", new { UserID = _userID }))
                {
                    var accounts = reader.Read<AccountViewModel>();
                    var categories = reader.Read<CategoryViewModel>();

                    foreach (var account in accounts)
                        account.Categories = categories.Where(c => c.AccountID == account.ID);

                    return new IndexViewModel {
                        Accounts = accounts
                    };
                }
            });

            return View(model);
        }

        public ActionResult NetWorth() => View("_NetWorth", new NetWorthViewModel {
            Accounts = _db.Query(conn => conn.QuerySP<AccountViewModel>("NetWorth", new { UserID = _userID }))
        });
    }
}
