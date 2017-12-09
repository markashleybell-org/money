﻿using System;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using money.common;

namespace money.web.Support
{
    public class AuthFilter : IAuthorizationFilter, IOverrideFilter
    {
        public Type FiltersToOverride => typeof(IAuthorizationFilter);

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var session = filterContext.HttpContext.Session[Globals.USER_SESSION_VARIABLE_NAME];

            if (session == null)
                filterContext.Result = new RedirectResult("/Users/Login");
        }
    }
}
