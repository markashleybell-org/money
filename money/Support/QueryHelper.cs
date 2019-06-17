using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper.Contrib.Extensions;
using Money.Entities;

namespace Money.Support
{
    public class QueryHelper : IQueryHelper
    {
        private readonly IUnitOfWork _unitOfWork;

        // TODO: Inject user ID, lock down queries
        public QueryHelper(IUnitOfWork unitOfWork) =>
            _unitOfWork = unitOfWork;

        public T Get<T>(int id)
            where T : class, IEntity => Query(conn => conn.Get<T>(id));

        public T Query<T>(Func<IDbConnection, T> body)
        {
            using (var conn = new SqlConnection(_unitOfWork.ConnectionString))
            {
                conn.Open();

                return body(conn);
            }
        }

        public IEnumerable<T> Query<T>(Func<IDbConnection, IEnumerable<T>> body)
        {
            using (var conn = new SqlConnection(_unitOfWork.ConnectionString))
            {
                conn.Open();

                return body(conn);
            }
        }

        public void Execute(Action<IDbConnection, IDbTransaction> body)
        {
            var transaction = _unitOfWork.GetTransaction();

            body(transaction.Connection, transaction);
        }

        public int InsertOrUpdate<T>(T dto)
            where T : class, IEntity
        {
            Execute((conn, transaction) =>
            {
                if (dto.ID == 0)
                {
                    conn.Insert(dto, transaction);
                }
                else
                {
                    conn.Update(dto, transaction);
                }
            });

            return dto.ID;
        }

        public void Delete<T>(T dto)
            where T : class, IEntity =>
                Execute((conn, transaction) => conn.Delete(dto, transaction));
    }
}
