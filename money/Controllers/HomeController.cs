using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using money.Models;
using money.Support;

namespace money.Controllers
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
            var model = _db.Query(conn =>
            {
                using (var reader = conn.QueryMultipleSP("Dashboard", new { UserID }))
                {
                    var netWorthAccounts = reader.Read<AccountViewModel>();
                    var accounts = reader.Read<AccountViewModel>();
                    var categories = reader.Read<CategoryViewModel>();

                    foreach (var account in accounts)
                    {
                        account.Categories = categories.Where(c => c.AccountID == account.ID);
                    }

                    return new IndexViewModel
                    {
                        NetWorthAccounts = netWorthAccounts,
                        Accounts = accounts.Where(a => !a.IsDormant)
                    };
                }
            });

            return View(model);
        }

        public ActionResult NetWorth()
        {
            var parameters = new
            {
                UserID,
                NonZeroBalanceOnly = true
            };

            var model = new NetWorthViewModel
            {
                Accounts = _db.Query(conn => conn.QuerySP<AccountViewModel>("AccountList", parameters))
            };

            return View("_NetWorth", model);
        }
    }
}
