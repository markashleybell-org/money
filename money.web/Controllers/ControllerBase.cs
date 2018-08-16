using System;
using System.Web.Mvc;
using money.common;
using money.web.Abstract;
using money.web.Support;

namespace money.web.Controllers
{
    [Auth]
    public class ControllerBase : Controller
    {
#pragma warning disable SA1401 // Fields must be private
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IQueryHelper _db;
        protected readonly IRequestContext _context;
        protected readonly int _userID;
#pragma warning restore SA1401 // Fields must be private

        public ControllerBase(
            IUnitOfWork unitOfWork,
            IQueryHelper db,
            IRequestContext context)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _context = context;

            var userID = _context.GetSessionItemValue(Globals.USER_SESSION_VARIABLE_NAME) as int?;

            _userID = userID ?? -1;
        }

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
