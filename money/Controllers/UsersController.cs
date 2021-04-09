using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Money.Models;
using Money.Support;
using static Money.Support.MvcExtensions;

namespace Money.Controllers
{
    public class UsersController : MoneyControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(
            IOptionsMonitor<Settings> optionsMonitor,
            IDateTimeService dateTimeService,
            IUnitOfWork unitOfWork,
            IQueryHelper db,
            IUserService userService)
            : base(
                  optionsMonitor,
                  dateTimeService,
                  unitOfWork,
                  db) =>
            _userService = userService;

        [AllowAnonymous]
        public IActionResult Login() =>
            View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (valid, id) = _userService.ValidateLogin(model.Email, model.Password);

            if (!valid)
            {
                return View(model);
            }

            var principal = _userService.GetClaimsPrincipal(id.Value, model.Email);

            var authenticationProperties = new AuthenticationProperties {
                IsPersistent = true,
                ExpiresUtc = DateTimeService.Now.AddDays(Cfg.PersistentSessionLengthInDays)
            };

            await HttpContext.SignInAsync(principal, authenticationProperties);

            return RedirectTo<HomeController>(c => c.Index());
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectTo<HomeController>(c => c.Index());
        }
    }
}
