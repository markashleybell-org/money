using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace money.web.Abstract
{
    public interface IQueryHelper
    {
        T Get<T>(int id) where T : class, IEntity;
        void Delete<T>(T dto) where T : class, IEntity;
        void Execute(Action<IDbConnection, IDbTransaction> body);
        int InsertOrUpdate<T>(T dto) where T : class, IEntity;
        IEnumerable<T> Query<T>(Func<IDbConnection, IDbTransaction, IEnumerable<T>> body);
        T Query<T>(Func<IDbConnection, IDbTransaction, T> body);
        IEnumerable<T> Query<T>(Func<IDbConnection, IEnumerable<T>> body);
        T Query<T>(Func<IDbConnection, T> body);
    }
}
