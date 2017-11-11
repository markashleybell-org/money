using money.web.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace money.web.Controllers
{
    public class HomeController : ControllerBase
    {
        public HomeController(IUnitOfWork unitOfWork, IQueryHelper db) : base(unitOfWork, db) { }

        public ActionResult Index()
        {
            return View();
        }
    }
}