using System;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using money.Support;
using money.Entities;

namespace money.web.Concrete
{
    public class PersistentSessionManager : IPersistentSessionManager
    {
#pragma warning disable SA1310 // Field names must not contain underscore
        private const int TOKEN_SIZE = 8;
#pragma warning restore SA1310 // Field names must not contain underscore

        private readonly IQueryHelper _db;
        private readonly int _sessionLengthInDays;

        public PersistentSessionManager(
            IQueryHelper db,
            int sessionLengthInDays)
        {
            _db = db;
            _sessionLengthInDays = sessionLengthInDays;
        }

        public PersistentSession CreatePersistentSession(int userID)
        {
            var seriesIdentifier = GetRandomNumber(TOKEN_SIZE);
            var token = GetRandomNumber(TOKEN_SIZE);

            var created = DateTime.Now;
            var expiry = created.AddDays(_sessionLengthInDays);

            var session = new PersistentSession(userID, seriesIdentifier, token, created, expiry);

            var sql = @"INSERT INTO 
                            PersistentSessions (
                                UserID,
                                SeriesIdentifier, 
                                Token,
                                Created,
                                Expires
                            ) 
                        VALUES (
                            @UserID, 
                            @SeriesIdentifier,
                            @Token,
                            @Created,
                            @Expires
                        )";

            _db.Execute((conn, tran) => conn.Execute(sql, session, tran));

            return session;
        }

        public PersistentSession GetPersistentSession(PersistentSession session)
        {
            var sql = @"SELECT 
                            * 
                        FROM 
                            PersistentSessions 
                        WHERE 
                            UserID = @UserID 
                        AND 
                            SeriesIdentifier = @SeriesIdentifier 
                        AND 
                            Token = @Token";

            return _db.Query(conn => conn.QuerySingleOrDefault<PersistentSession>(sql, session));
        }

        public PersistentSession UpdatePersistentSession(PersistentSession session)
        {
            var token = GetRandomNumber(TOKEN_SIZE);

            var updatedPersistentSession = GetPersistentSession(session).WithUpdates(token);

            var parameters = new {
                updatedPersistentSession.UserID,
                updatedPersistentSession.SeriesIdentifier,
                updatedPersistentSession.Token,
                ExistingToken = session.Token
            };

            var sql = @"UPDATE 
                            PersistentSessions 
                        SET 
                            Token = @Token 
                        WHERE 
                            UserID = @UserID 
                        AND 
                            SeriesIdentifier = @SeriesIdentifier
                        AND 
                            Token = @ExistingToken";

            _db.Execute((conn, tran) => conn.Execute(sql, parameters, tran));

            return updatedPersistentSession;
        }

        public void DestroyPersistentSession(int userID, string seriesIdentifier)
        {
            var param = new {
                UserID = userID,
                SeriesIdentifier = seriesIdentifier
            };

            var sql = @"DELETE FROM 
                            PersistentSessions 
                        WHERE 
                            UserID = @UserID 
                        AND 
                            SeriesIdentifier = @SeriesIdentifier";

            _db.Execute((conn, tran) => conn.Execute(sql, param, tran));
        }

        private string GetRandomNumber(int size)
        {
            var data = new byte[size];

            RandomNumberGenerator.Create().GetBytes(data);

            return BitConverter.ToUInt64(data, 0).ToString();
        }
    }
}
