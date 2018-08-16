using System;
using System.Web;
using money.web.Abstract;

namespace money.web.Concrete
{
    public class RequestContext : IRequestContext
    {
        private HttpContextBase _context;

        public RequestContext(HttpContextBase context) =>
            _context = context;

        public string RequestUrl =>
            _context.Request.Url.AbsolutePath;

        public string GetCookieValue(string name) =>
            _context.Request.Cookies[name]?.Value;

        public void SetCookie(string name, string value, DateTime? expires = null)
        {
            var cookie = _context.Response.Cookies[name];

            cookie.HttpOnly = true;
            cookie.Secure = true;
            cookie.Value = value;

            if (expires.HasValue)
            {
                cookie.Expires = expires.Value;
            }
        }

        public void DeleteCookie(string name) =>
            _context.Response.Cookies[name].Expires = DateTime.Now.AddDays(-1);

        public object GetSessionItemValue(string name) =>
            _context.Session[name];

        public void SetSessionItem(string name, object value) =>
            _context.Session[name] = value;

        public void DeleteSessionItem(string name) =>
            _context.Session[name] = null;
    }
}
