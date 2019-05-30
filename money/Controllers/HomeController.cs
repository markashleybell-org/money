using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using money.Entities;
using money.Models;
using money.Support;

namespace money.Controllers 
{
    public class HomeController : ControllerBase 
    {
        public HomeController(
            IOptionsMonitor<Settings> optionsMonitor,
            IUnitOfWork unitOfWork,
            IQueryHelper db,
            IRequestContext ctx)
            : base(
                  optionsMonitor,
                  unitOfWork,
                  db,
                  ctx)
        {
        }

        public IActionResult Index()
        {
            var testData = _db.Get<Category>(1);

            var model = new IndexViewModel {
                RequestContext = _ctx,
                UnitOfWork = _unitOfWork,
                TestData = testData.Name 
            };

            return View(model);
        }
    }
}
