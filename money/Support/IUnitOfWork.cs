using System.Data;

namespace money.Support
{
    public interface IUnitOfWork
    {
        string ConnectionString { get; }

        IDbTransaction GetTransaction();

        void CommitChanges();
    }
}
