using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using money.Controllers;
using money.Entities;
using money.Models;
using money.Support;

namespace money.web.Controllers
{
    public class UsersController : MoneyControllerBase
    {
        private readonly IPersistentSessionManager _persistentSessionManager;

        public UsersController(
            IOptionsMonitor<Settings> optionsMonitor,
            IUnitOfWork unitOfWork,
            IQueryHelper db,
            IRequestContext ctx,
            IPersistentSessionManager persistentSessionManager)
            : base(
                  optionsMonitor,
                  unitOfWork,
                  db,
                  ctx)
            => _persistentSessionManager = persistentSessionManager;

        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sql = "SELECT * FROM Users WHERE Email = @Email";

            var user = _db.Query(conn => conn.QuerySingleOrDefault<User>(sql, new { model.Email }));

            if (user == null)
            {
                return View(model);
            }

            var hasher = new PasswordHasher<User>();
            var verificationResult = hasher.VerifyHashedPassword(user, user.Password, model.Password);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                return View(model);
            }

            _ctx.SetSessionItem(Globals.USER_SESSION_VARIABLE_NAME, user.ID);

            DestroyCurrentPersistentSession();

            var persistentSession = _persistentSessionManager.CreatePersistentSession(user.ID);

            _unitOfWork.CommitChanges();

            _ctx.SetCookie(
                Globals.PERSISTENT_LOGIN_COOKIE_NAME,
                persistentSession.AsCookieString(),
                persistentSession.Expires
            );

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            _ctx.DeleteSessionItem(Globals.USER_SESSION_VARIABLE_NAME);

            DestroyCurrentPersistentSession();

            _unitOfWork.CommitChanges();

            return RedirectToAction("Index", "Home");
        }

        private void DestroyCurrentPersistentSession()
        {
            var persistentSessionData = _ctx.GetCookieValue(Globals.PERSISTENT_LOGIN_COOKIE_NAME);

            if (persistentSessionData != null)
            {
                var persistentSession = persistentSessionData.AsPersistentSession();

                _persistentSessionManager.DestroyPersistentSession(persistentSession.UserID, persistentSession.SeriesIdentifier);
            }

            _ctx.DeleteCookie(Globals.PERSISTENT_LOGIN_COOKIE_NAME);
        }
    }
}
