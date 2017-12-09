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
        public UsersController(IUnitOfWork unitOfWork, IQueryHelper db, IRequestContext context) : base(unitOfWork, db, context) { }

        [OverrideAuthorization]
        public ActionResult Login() => View();

        [HttpPost]
        [OverrideAuthorization]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _db.Query(conn => conn.QuerySingleOrDefault<User>("SELECT * FROM Users WHERE Email = @Email", new { model.Email }));

            if (user == null)
                return View(model);

            var hasher = new PasswordHasher();
            var verificationResult = hasher.VerifyHashedPassword(user.Password, model.Password);

            if (verificationResult != PasswordVerificationResult.Success)
                return View(model);

            _context.SetSessionItem(Globals.USER_SESSION_VARIABLE_NAME, user.ID);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            _context.DeleteSessionItem(Globals.USER_SESSION_VARIABLE_NAME);

            return RedirectToAction("Index", "Home");
        }
    }
}
