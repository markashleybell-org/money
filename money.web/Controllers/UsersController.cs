using System.Web.Mvc;
using Dapper;
using Microsoft.AspNet.Identity;
using money.common;
using money.web.Abstract;
using money.web.Models;
using money.web.Models.Entities;

namespace money.web.Controllers
{
    public class UsersController : ControllerBase
    {
        private IPersistentSessionManager _persistentSessionManager;

        public UsersController(
            IUnitOfWork unitOfWork,
            IQueryHelper db,
            IRequestContext context,
            IPersistentSessionManager persistentSessionManager)
            : base(unitOfWork, db, context) =>
            _persistentSessionManager = persistentSessionManager;

        [OverrideAuthorization]
        public ActionResult Login() => View();

        [HttpPost]
        [OverrideAuthorization]
        public ActionResult Login(LoginViewModel model)
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

            var hasher = new PasswordHasher();
            var verificationResult = hasher.VerifyHashedPassword(user.Password, model.Password);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                return View(model);
            }

            _context.SetSessionItem(Globals.USER_SESSION_VARIABLE_NAME, user.ID);

            DestroyCurrentPersistentSession();

            var persistentSession = _persistentSessionManager.CreatePersistentSession(user.ID);

            _unitOfWork.CommitChanges();

            _context.SetCookie(
                Globals.PERSISTENT_LOGIN_COOKIE_NAME,
                persistentSession.AsCookieString(),
                persistentSession.Expires
            );

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            _context.DeleteSessionItem(Globals.USER_SESSION_VARIABLE_NAME);

            DestroyCurrentPersistentSession();

            _unitOfWork.CommitChanges();

            return RedirectToAction("Index", "Home");
        }

        private void DestroyCurrentPersistentSession()
        {
            var persistentSessionData = _context.GetCookieValue(Globals.PERSISTENT_LOGIN_COOKIE_NAME);

            if (persistentSessionData != null)
            {
                var persistentSession = persistentSessionData.AsPersistentSession();

                _persistentSessionManager.DestroyPersistentSession(persistentSession.UserID, persistentSession.SeriesIdentifier);
            }

            _context.DeleteCookie(Globals.PERSISTENT_LOGIN_COOKIE_NAME);
        }
    }
}
