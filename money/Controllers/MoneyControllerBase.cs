using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using money.Support;

namespace money.Controllers
{
    public abstract class MoneyControllerBase : Controller
    {
        protected readonly Settings _cfg;

        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IQueryHelper _db;
        protected readonly IRequestContext _ctx;

        protected MoneyControllerBase(
            IOptionsMonitor<Settings> optionsMonitor,
            IUnitOfWork unitOfWork,
            IQueryHelper db,
            IRequestContext ctx)
        {
            _cfg = optionsMonitor.CurrentValue;

            _unitOfWork = unitOfWork;
            _db = db;
            _ctx = ctx;
        }

        // TODO: Wire up auth
        protected int UserID => 1;
            // _ctx.GetSessionItemValue(Globals.USER_SESSION_VARIABLE_NAME) as int? ?? -1;

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && _unitOfWork is IDisposable uowDisposable)
        //    {
        //        uowDisposable.Dispose();
        //    }

        //    base.Dispose(disposing);
        //}
    }
}
