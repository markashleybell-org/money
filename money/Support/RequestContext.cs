using System;
using Microsoft.AspNetCore.Http;

namespace money.Support
{
    public class RequestContext : IRequestContext
    {
        private readonly HttpContext _ctx;

        public RequestContext(IHttpContextAccessor httpContextAccessor) =>
            _ctx = httpContextAccessor.HttpContext;

        public string RequestUrl =>
            _ctx.Request.Path;

        public string GetCookieValue(string key) =>
            _ctx.Request.Cookies.TryGetValue(key, out var val) ? val : default;

        public void SetCookie(string key, string value, DateTime? expires = null)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true
            };

            if (expires.HasValue)
            {
                options.Expires = expires.Value;
            }

            _ctx.Response.Cookies.Append(key, value, options);
        }

        public void DeleteCookie(string key) =>
            _ctx.Response.Cookies.Delete(key);

        public object GetSessionItemValue(string key) =>
            _ctx.Session.Get(key);

        public void SetSessionItem(string key, int value) =>
            _ctx.Session.SetInt32(key, value);

        public void DeleteSessionItem(string key) =>
            _ctx.Session.Remove(key);
    }
}
