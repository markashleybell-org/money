using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Money.Support
{
    public class ViewRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ViewRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Render(string viewPath) =>
            await Render(viewPath, default(string));

        public async Task<string> Render<TModel>(string viewPath, TModel model)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var routeData = httpContext.GetRouteData();

            var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());

            var viewEngineResult = _viewEngine.FindView(actionContext, viewPath, false);

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException($"Couldn't find view {viewPath}");
            }

            using (var output = new StringWriter())
            {
                var view = viewEngineResult.View;

                var viewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var tempData = new TempDataDictionary(httpContext, _tempDataProvider);

                var viewContext = new ViewContext(actionContext, view, viewData, tempData, output, new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }
    }
}
