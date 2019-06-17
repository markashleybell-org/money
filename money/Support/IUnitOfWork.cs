using System.Data;

namespace Money.Support
{
    public interface IUnitOfWork
    {
        string ConnectionString { get; }

        IDbTransaction GetTransaction();

        void CommitChanges();
    }
}
