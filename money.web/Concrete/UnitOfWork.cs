using System;
using System.Data;
using System.Data.SqlClient;
using money.web.Abstract;

namespace money.web.Concrete
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private string _connectionString;
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public UnitOfWork(string connectionString) => _connectionString = connectionString;

        public IDbTransaction GetTransaction()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }

            if (_transaction == null)
                _transaction = _connection.BeginTransaction();

            return _transaction;
        }

        public void CommitChanges()
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.Commit();
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    Dispose();
                }
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
