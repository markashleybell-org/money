using Dapper.Contrib.Extensions;
using money.web.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace money.web.Concrete
{
    public class QueryHelper : IQueryHelper
    {
        private IUnitOfWork _unitOfWork;
        private string _connectionString;

        public QueryHelper(IUnitOfWork unitOfWork, string connectionString)
        {
            _unitOfWork = unitOfWork;
            _connectionString = connectionString;
        }

        public T Get<T>(int id)
             where T : class, IDTO
        {
            return Query(conn => conn.Get<T>(id));
        }

        public T Query<T>(Func<IDbConnection, T> body)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                return body(conn);
            }
        }

        public IEnumerable<T> Query<T>(Func<IDbConnection, IEnumerable<T>> body)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                return body(conn);
            }
        }

        public T Query<T>(Func<IDbConnection, IDbTransaction, T> body)
        {
            var transaction = _unitOfWork.GetTransaction();
            return body(transaction.Connection, transaction);
        }

        public IEnumerable<T> Query<T>(Func<IDbConnection, IDbTransaction, IEnumerable<T>> body)
        {
            var transaction = _unitOfWork.GetTransaction();
            return body(transaction.Connection, transaction);
        }

        public void Execute(Action<IDbConnection, IDbTransaction> body)
        {
            var transaction = _unitOfWork.GetTransaction();
            body(transaction.Connection, transaction);
        }

        public int InsertOrUpdate<T>(T dto)
            where T : class, IDTO
        {
            Execute((conn, transaction) => {
                if (dto.ID == 0)
                    conn.Insert(dto, transaction);
                else
                    conn.Update(dto, transaction);
            });

            return dto.ID;
        }

        public void Delete<T>(T dto)
            where T : class, IDTO
        {
            Execute((conn, transaction) => conn.Delete(dto, transaction));
        }
    }
}