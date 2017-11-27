using money.web.Abstract;
using money.web.Models;
using money.web.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

                    foreach(var account in accounts)
                        account.Categories = categories.Where(c => c.AccountID == account.ID);

                    return new IndexViewModel {
                        Accounts = accounts
                    };
                }
            });

            return View(model);
        }
    }
}