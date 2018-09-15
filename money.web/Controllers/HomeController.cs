using System.Linq;
using System.Web.Mvc;
using money.web.Abstract;
using money.web.Models;
using money.web.Support;

namespace money.web.Controllers
{
    public class HomeController : ControllerBase
    {
        public HomeController(
            IUnitOfWork unitOfWork,
            IQueryHelper db,
            IRequestContext context)
            : base(
                  unitOfWork,
                  db,
                  context)
        { }

        public ActionResult Index()
        {
            var model = _db.Query(conn => {
                using (var reader = conn.QueryMultipleSP("Dashboard", new { UserID }))
                {
                    var netWorthAccounts = reader.Read<AccountViewModel>();
                    var accounts = reader.Read<AccountViewModel>();
                    var categories = reader.Read<CategoryViewModel>();

                    foreach (var account in accounts)
                    {
                        account.Categories = categories.Where(c => c.AccountID == account.ID);
                    }

                    return new IndexViewModel {
                        NetWorthAccounts = netWorthAccounts,
                        Accounts = accounts.Where(a => !a.IsDormant)
                    };
                }
            });

            return View(model);
        }

        public ActionResult NetWorth()
        {
            var parameters = new {
                UserID,
                NonZeroBalanceOnly = true
            };

            var model = new NetWorthViewModel {
                Accounts = _db.Query(conn => conn.QuerySP<AccountViewModel>("AccountList", parameters))
            };

            return View("_NetWorth", model);
        }
    }
}
