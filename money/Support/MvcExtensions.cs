using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Money.Entities;
using static Money.Support.Globals;

namespace Money.Support
{
    public static class MvcExtensions
    {
        private static Func<Expression, object> _convertExpression =>
            (Expression e) => {
                switch (e)
                {
                    case ConstantExpression ce:
                        return ce.Value;
                    case MemberExpression me:
                        return me.GetValue();
                    case UnaryExpression ue:
                        return (ue.Operand as MemberExpression).GetValue();
                }

                return null;
            };

        public static string GetDisplayName(this Enum enumValue) =>
            enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DisplayAttribute>().Name;

        public static ActionResult RedirectTo<T>(Expression<Func<T, IActionResult>> action)
            where T : ControllerBase =>
                new RedirectToRouteResult(GetRouteValuesFor(action));

        public static IEnumerable<SelectListItem> TypesSelectListItems(EntryType entryTypes, Func<IEnumerable<Account>> accountList)
        {
            var types = Enum.GetNames(typeof(EntryType)).Where(n => n != EntryType.Transfer.ToString())
                .Select(n => new SelectListItem { Value = n, Text = n }).ToList();

            if (entryTypes.HasFlag(EntryType.Transfer))
            {
                var accounts = accountList()
                    .Where(a => !a.IsDormant)
                    .Select(a => new SelectListItem
                    {
                        Value = $"Transfer-{a.ID}",
                        Text = $"Transfer to {a.Name}"
                    });

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

        private static RouteValueDictionary GetRouteValuesFor<T>(Expression<Func<T, IActionResult>> action)
            where T : ControllerBase
        {
            if (!(action.Body is MethodCallExpression methodCallExpression)
                || !methodCallExpression.Method.ReturnType.IsAssignableFrom(typeof(IActionResult)))
            {
                throw new ArgumentException("Redirect action must be a method call which returns IActionResult", nameof(action));
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
