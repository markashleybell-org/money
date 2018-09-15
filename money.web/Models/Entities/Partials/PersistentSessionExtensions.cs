using System;
using static money.common.Globals;

namespace money.web.Models.Entities
{
    public static class PersistentSessionExtensions
    {
        private const string PersistentCookieValueParseErrorMessage = "Could not parse persistent session cookie value";

        public static PersistentSession WithUpdates(this PersistentSession session, string token) =>
            new PersistentSession(
                session.UserID,
                session.SeriesIdentifier,
                token,
                session.Created,
                session.Expires
            );

        public static string AsCookieString(this PersistentSession session) =>
            $"{session.UserID}|{session.SeriesIdentifier}|{session.Token}|{session.Created.ToString(COOKIE_DATE_FORMAT)}|{session.Expires.ToString(COOKIE_DATE_FORMAT)}";

        public static PersistentSession AsPersistentSession(this string cookieValue)
        {
            if (string.IsNullOrWhiteSpace(cookieValue))
            {
                throw new ArgumentException(nameof(cookieValue), PersistentCookieValueParseErrorMessage);
            }

            var parts = cookieValue.Split('|');

            if (parts.Length != 5)
            {
                throw new ArgumentException(nameof(cookieValue), PersistentCookieValueParseErrorMessage);
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
                throw new ArgumentException(nameof(cookieValue), PersistentCookieValueParseErrorMessage, ex);
            }
        }
    }
}
