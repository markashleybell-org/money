using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using money.web.Abstract;
using money.web.Concrete;
using money.web.Support;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using Ninject.Web.Mvc.FilterBindingSyntax;

namespace money.web
{
    public class MvcApplication : NinjectHttpApplication
    {
        protected override void OnApplicationStarted()
        {
            AreaRegistration.RegisterAllAreas();
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

            kernel.Bind<IRequestContext>()
                .To<Concrete.RequestContext>()
                .InRequestScope();

            kernel.BindFilter<AuthFilter>(FilterScope.Controller, 0)
                .WhenControllerHas<AuthAttribute>();

            return kernel;
        }
    }
}
