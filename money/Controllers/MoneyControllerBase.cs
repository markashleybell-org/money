using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Money.Support;

namespace Money.Controllers
{
    [Authorize]
    public abstract class MoneyControllerBase : Controller
    {
        protected MoneyControllerBase(
            IOptionsMonitor<Settings> optionsMonitor,
            IUnitOfWork unitOfWork,
            IQueryHelper db)
        {
            Cfg = optionsMonitor.CurrentValue;
            UnitOfWork = unitOfWork;
            Db = db;
        }

        protected Settings Cfg { get; }

        protected IUnitOfWork UnitOfWork { get; }

        protected IQueryHelper Db { get; }

        protected int UserID =>
            Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value);

        protected override void Dispose(bool disposing)
        {
            if (disposing && UnitOfWork is IDisposable uowDisposable)
            {
                uowDisposable.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
