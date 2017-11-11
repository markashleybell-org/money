using money.web.Abstract;
using money.web.Concrete;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace money.web
{
    public class MvcApplication : NinjectHttpApplication
    {
        protected override void OnApplicationStarted()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            var connectionString = ConfigurationManager.ConnectionStrings["Main"].ConnectionString;

            kernel.Bind<IUnitOfWork>()
                .To<UnitOfWork>()
                .InRequestScope()
                .WithConstructorArgument("connectionString", connectionString);

            kernel.Bind<IQueryHelper>()
                .To<QueryHelper>()
                .InRequestScope()
                .WithConstructorArgument("connectionString", connectionString);

            return kernel;
        }
    }
}
