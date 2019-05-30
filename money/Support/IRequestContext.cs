using System;

namespace money.Support
{
    public interface IRequestContext
    {
        string RequestUrl { get; }

        string GetCookieValue(string key);

        void SetCookie(string key, string value, DateTime? expires = null);

        void DeleteCookie(string key);

        object GetSessionItemValue(string key);

        void SetSessionItem(string key, int value);

        void DeleteSessionItem(string key);
    }
}
