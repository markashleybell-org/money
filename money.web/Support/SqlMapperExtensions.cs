using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static Dapper.SqlMapper;

namespace money.web.Support
{
    public static partial class SqlMapperExtensions
    {
        public static IEnumerable<T> QuerySP<T>(this IDbConnection connection, string sql, object param, IDbTransaction transaction = null)
        {
            return connection.Query<T>(sql, param, transaction, true, null, CommandType.StoredProcedure);
        }

        public static GridReader QueryMultipleSP(this IDbConnection connection, string sql, object param)
        {
            return connection.QueryMultiple(sql, param, null, null, CommandType.StoredProcedure);
        }

        public static void ExecuteSP(this IDbConnection connection, string sql, object param, IDbTransaction transaction = null)
        {
            connection.Execute(sql, param, transaction, null, CommandType.StoredProcedure);
        }
    }
}