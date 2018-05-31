using System.Collections.Generic;
using System.Linq;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class NetWorthViewModel
    {
        public NetWorthViewModel() => Accounts = new List<AccountViewModel>();

        public decimal NetWorth => Accounts.Where(a => a.IsIncludedInNetWorth).Sum(a => a.CurrentBalance);

        public IEnumerable<AccountViewModel> Accounts { get; set; }
    }
}
