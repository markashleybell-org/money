using System.Data;

namespace money.web.Abstract
{
    public interface IUnitOfWork
    {
        IDbTransaction GetTransaction();
        void CommitChanges();
    }
}
