using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Dapper;
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
            where T : class, IEntity =>
            Query(conn => conn.Get<T>(id));

        public T Query<T>(Func<IDbConnection, T> body)
        {
            using var conn = new SqlConnection(_unitOfWork.ConnectionString);

            conn.Open();

            return body(conn);
        }

        public IEnumerable<T> Query<T>(Func<IDbConnection, IEnumerable<T>> body)
        {
            using var conn = new SqlConnection(_unitOfWork.ConnectionString);

            conn.Open();

            return body(conn);
        }

        public void Execute(Action<IDbConnection, IDbTransaction> body)
        {
            var transaction = _unitOfWork.GetTransaction();

            body(transaction.Connection, transaction);
        }

        public int InsertOrUpdate<T>(T dto)
            where T : class, IEntity
        {
            Execute((conn, transaction) => {
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
                Execute((conn, transaction) => {
                    if (dto is ISoftDeletable<T> d)
                    {
                        conn.Update(d.ForDeletion(), transaction);
                    }
                    else
                    {
                        conn.Delete(dto, transaction);
                    }
                });

        public void Undelete<T>(T dto)
            where T : class, IEntity, ISoftDeletable<T> =>
                Execute((conn, transaction) => conn.Update(dto.ForUndeletion(), transaction));

        public void UpdateDisplayOrder<T>(IEnumerable<int> order)
            where T : class, IEntity, IOrderable<T>
        {
            var tableAttribute = typeof(T).GetCustomAttribute<TableAttribute>(inherit: true);

            var keyProperty = typeof(T).GetProperties()
                .SingleOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));

            if (tableAttribute is null)
            {
                throw new Exception($"{typeof(T).FullName} does not have a table attribute specifying which table to update.");
            }

            if (keyProperty is null)
            {
                throw new Exception($"{typeof(T).FullName} does not have a primary key property.");
            }

            Execute((conn, transaction) => {
                var updates = order.Select((id, i) => $"UPDATE {tableAttribute.Name} SET DisplayOrder = {i} WHERE {keyProperty.Name} = {id}");
                conn.Execute($"UPDATE {tableAttribute.Name} SET DisplayOrder = -1 WHERE Deleted = 1; " + string.Join("; ", updates), transaction: transaction);
            });
        }
    }
}
