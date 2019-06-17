using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Money.Models;
using Money.Support;

namespace Money.Controllers
{
    public class HomeController : MoneyControllerBase
    {
        public HomeController(
            IOptionsMonitor<Settings> optionsMonitor,
            IUnitOfWork unitOfWork,
            IQueryHelper db)
            : base(
                  optionsMonitor,
                  unitOfWork,
                  db)
        {
        }

        public IActionResult Index()
        {
            var model = Db.Query(conn => {
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
                        Accounts = accounts.Where(a => !a.IsDormant),
                        NetWorthViewModel = new NetWorthViewModel {
                            Accounts = netWorthAccounts
                        }
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
                Accounts = Db.Query(conn => conn.QuerySP<AccountViewModel>("AccountList", parameters))
            };

            return View("_NetWorth", model);
        }
    }
}
