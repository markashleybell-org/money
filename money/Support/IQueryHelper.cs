using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Money.Entities;

namespace Money.Support
{
    public interface IQueryHelper
    {
        T Get<T>(int id)
            where T : class, IEntity;

        void Delete<T>(T dto)
            where T : class, IEntity;

        void Execute(Action<IDbConnection, IDbTransaction> body);

        int InsertOrUpdate<T>(T dto)
            where T : class, IEntity;

        IEnumerable<T> Query<T>(Func<IDbConnection, IEnumerable<T>> body);

        T Query<T>(Func<IDbConnection, T> body);
    }
}
