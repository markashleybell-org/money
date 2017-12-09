using System.Collections.Generic;
using System.Data;
using Dapper;
using static Dapper.SqlMapper;

namespace money.web.Support
{
    public static partial class SqlMapperExtensions
    {
        public static IEnumerable<T> QuerySP<T>(this IDbConnection connection, string sql, object param, IDbTransaction transaction = null) =>
            connection.Query<T>(sql, param, transaction, true, null, CommandType.StoredProcedure);

        public static GridReader QueryMultipleSP(this IDbConnection connection, string sql, object param) =>
            connection.QueryMultiple(sql, param, null, null, CommandType.StoredProcedure);

        public static void ExecuteSP(this IDbConnection connection, string sql, object param, IDbTransaction transaction = null) =>
            connection.Execute(sql, param, transaction, null, CommandType.StoredProcedure);
    }
}
