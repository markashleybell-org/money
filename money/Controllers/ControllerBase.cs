using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using money.Support;

namespace money.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected readonly Settings _cfg;

        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IQueryHelper _db;
        protected readonly IRequestContext _ctx;

        protected ControllerBase(
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
    }
}
