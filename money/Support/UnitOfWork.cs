using System;
using System.Data;
using System.Data.SqlClient;

namespace money.Support
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public UnitOfWork(string connectionString) =>
            ConnectionString = connectionString;

        public string ConnectionString { get; set; }

        public IDbTransaction GetTransaction()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(ConnectionString);

                _connection.Open();
            }

            if (_transaction == null)
            {
                _transaction = _connection.BeginTransaction();
            }

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
                catch
                {
                    _transaction.Rollback();

                    throw;
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
