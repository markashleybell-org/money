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

        public ControllerBase(IUnitOfWork unitOfWork, IQueryHelper db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
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