using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.Entities
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
    }
}
