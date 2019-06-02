using System;
using money.Entities;
using static money.Support.Globals;

namespace money.Support
{
    public static class PersistentSessionExtensions
    {
        public static PersistentSession AsPersistentSession(this string cookieValue)
        {
            if (string.IsNullOrWhiteSpace(cookieValue))
            {
                throw new ArgumentException(nameof(cookieValue), PERSISTENT_COOKIE_VALUE_PARSE_ERROR_MESSAGE);
            }

            var parts = cookieValue.Split('|');

            if (parts.Length != 5)
            {
                throw new ArgumentException(nameof(cookieValue), PERSISTENT_COOKIE_VALUE_PARSE_ERROR_MESSAGE);
            }

            try
            {
                return new PersistentSession(
                    userId: Convert.ToInt32(parts[0]),
                    seriesIdentifier: parts[1],
                    token: parts[2],
                    created: DateTime.ParseExact(parts[3], COOKIE_DATE_FORMAT, null),
                    expires: DateTime.ParseExact(parts[4], COOKIE_DATE_FORMAT, null)
                );
            }
            catch (Exception ex)
            {
                throw new ArgumentException(nameof(cookieValue), PERSISTENT_COOKIE_VALUE_PARSE_ERROR_MESSAGE, ex);
            }
        }
    }
}
