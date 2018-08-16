using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using money.web.Models.Entities;
using static money.common.Globals;

namespace money.web.Support
{
    public static class Extensions
    {
        private static Func<Expression, object> _convertExpression =>
            (Expression e) => {
                if (e is ConstantExpression)
                {
                    return (e as ConstantExpression).Value;
                }

                if (e is MemberExpression)
                {
                    return (e as MemberExpression).GetValue();
                }

                return null;
            };

        public static string GetDisplayName(this Enum enumValue) =>
            enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DisplayAttribute>().Name;

        public static ActionResult RedirectTo<T>(Expression<Func<T, ActionResult>> action)
            where T : ControllerBase =>
                new RedirectToRouteResult(GetRouteValuesFor(action));

        public static string Url<T>(Expression<Func<T, ActionResult>> action)
            where T : ControllerBase =>
                new UrlHelper(HttpContext.Current.Request.RequestContext).RouteUrl(GetRouteValuesFor(action));

        public static IEnumerable<SelectListItem> TypesSelectListItems(EntryType entryTypes, Func<IEnumerable<Account>> accountList)
        {
            var types = Enum.GetNames(typeof(EntryType)).Where(n => n != EntryType.Transfer.ToString())
                    .Select(n => new SelectListItem { Value = n, Text = n }).ToList();

            if (entryTypes.HasFlag(EntryType.Transfer))
            {
                var accounts = accountList()
                   .Select(a => new SelectListItem { Value = $"Transfer-{a.ID}", Text = $"Transfer to {a.Name}" });

                types.AddRange(accounts);
            }

            if (!entryTypes.HasFlag(EntryType.Debit))
            {
                types.RemoveAll(t => t.Value == EntryType.Debit.ToString());
            }

            if (!entryTypes.HasFlag(EntryType.Credit))
            {
                types.RemoveAll(t => t.Value == EntryType.Credit.ToString());
            }

            return types;
        }

        private static RouteValueDictionary GetRouteValuesFor<T>(Expression<Func<T, ActionResult>> action)
            where T : ControllerBase
        {
            var methodCallExpression = action.Body as MethodCallExpression;

            if (methodCallExpression == null || methodCallExpression.Method.ReturnType != typeof(ActionResult))
            {
                throw new ArgumentException("Redirect action must be a method call which returns an ActionResult", nameof(action));
            }

            var controllerName = methodCallExpression.Object.Type.Name.Replace(CONTROLLER_SUFFIX, string.Empty);
            var methodName = methodCallExpression.Method.Name;

            var parameters = methodCallExpression.Method.GetParameters().Select(p => p.Name).ToArray();
            var arguments = methodCallExpression.Arguments.Select(_convertExpression).ToArray();

            var routeValues = new RouteValueDictionary {
                { "controller", controllerName },
                { "action", methodName }
            };

            for (var i = 0; i < parameters.Length; i++)
            {
                routeValues.Add(parameters[i], arguments[i]);
            }

            return routeValues;
        }

        private static object GetValue(this MemberExpression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();

            return getter();
        }
    }
}
