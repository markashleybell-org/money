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
        protected readonly Settings _cfg;

        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IQueryHelper _db;

        protected MoneyControllerBase(
            IOptionsMonitor<Settings> optionsMonitor,
            IUnitOfWork unitOfWork,
            IQueryHelper db)
        {
            _cfg = optionsMonitor.CurrentValue;

            _unitOfWork = unitOfWork;
            _db = db;
        }

        protected int UserID =>
            Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value);

        protected override void Dispose(bool disposing)
        {
            if (disposing && _unitOfWork is IDisposable uowDisposable)
            {
                uowDisposable.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
