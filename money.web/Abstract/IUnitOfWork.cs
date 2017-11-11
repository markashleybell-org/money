using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace money.web.Abstract
{
    public interface IUnitOfWork
    {
        IDbTransaction GetTransaction();
        void CommitChanges();
    }
}
