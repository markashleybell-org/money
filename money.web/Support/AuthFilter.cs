using System;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using money.common;
using money.web.Abstract;
using money.web.Concrete;
using money.web.Models.Entities;

namespace money.web.Support
{
    public class AuthFilter : IAuthorizationFilter, IOverrideFilter
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryHelper _db;
        private readonly IPersistentSessionManager _persistentSessionManager;

        public AuthFilter(
            IUnitOfWork unitOfWork,
            IQueryHelper db,
            IPersistentSessionManager persistentSessionManager)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _persistentSessionManager = persistentSessionManager;
        }

        public Type FiltersToOverride => typeof(IAuthorizationFilter);

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var context = new RequestContext(filterContext.HttpContext);

            var session = context.GetSessionItemValue(Globals.USER_SESSION_VARIABLE_NAME);

            if (session == null)
            {
                var persistentSessionData = context.GetCookieValue(Globals.PERSISTENT_LOGIN_COOKIE_NAME);

                if (persistentSessionData != null)
                {
                    var persistentSessionFromCookie = persistentSessionData.AsPersistentSession();

                    var persistentSession = _persistentSessionManager.GetPersistentSession(persistentSessionFromCookie);

                    if (persistentSession != null)
                    {
                        var updatedSession = _persistentSessionManager.UpdatePersistentSession(persistentSession);

                        _unitOfWork.CommitChanges();

                        context.SetCookie(
                            Globals.PERSISTENT_LOGIN_COOKIE_NAME,
                            updatedSession.AsCookieString(),
                            updatedSession.Expires
                        );

                        context.SetSessionItem(Globals.USER_SESSION_VARIABLE_NAME, updatedSession.UserID);

                        return;
                    }
                }

                var urlHelper = new UrlHelper(filterContext.RequestContext);

                filterContext.Result = new RedirectResult(urlHelper.Action("Login", "Users"));
            }
        }
    }
}
