using money.common;
using money.web.Abstract;
using money.web.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace money.web.Controllers
{
    [Auth]
    public class ControllerBase : Controller
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IQueryHelper _db;
        protected readonly IRequestContext _context;
        protected readonly int _userID;

        public ControllerBase(IUnitOfWork unitOfWork, IQueryHelper db, IRequestContext context)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _context = context;

            var userID = _context.GetSessionItemValue(Globals.USER_SESSION_VARIABLE_NAME) as int?;

            _userID = userID.HasValue ? userID.Value : -1;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var uowDisposable = _unitOfWork as IDisposable;

                if (uowDisposable != null)
                    uowDisposable.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}