using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace money.web.Abstract
{
    public interface IRequestContext
    {
        string RequestUrl { get; }
        string GetCookieValue(string name);
        void SetCookie(string name, string value, DateTime? expires = null);
        void DeleteCookie(string name);
        object GetSessionItemValue(string name);
        void SetSessionItem(string name, object value);
        void DeleteSessionItem(string name);
    }
}