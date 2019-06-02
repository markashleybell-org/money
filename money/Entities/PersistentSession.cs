using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;
using static money.Support.Globals;

namespace money.Entities
{
    [d.Table("PersistentSessions")]
    public partial class PersistentSession
    {
        public PersistentSession(
            int userId,
            string seriesIdentifier,
            string token,
            DateTime created,
            DateTime expires)
        {
            UserID = userId;
            SeriesIdentifier = seriesIdentifier;
            Token = token;
            Created = created;
            Expires = expires;
        }

        public int UserID { get; private set; }

        [StringLength(256)]
        public string SeriesIdentifier { get; private set; }

        [StringLength(256)]
        public string Token { get; private set; }

        public DateTime Created { get; private set; }

        public DateTime Expires { get; private set; }

        public PersistentSession WithUpdates(
            string token)
            => new PersistentSession(
                UserID,
                SeriesIdentifier,
                token,
                Created,
                Expires
            );

        public string AsCookieString() =>
            $"{UserID}|{SeriesIdentifier}|{Token}|{Created.ToString(COOKIE_DATE_FORMAT)}|{Expires.ToString(COOKIE_DATE_FORMAT)}";
    }
}
