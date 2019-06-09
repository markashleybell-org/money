using System.Collections.Generic;
using System.Linq;

namespace money.Models
{
    public class NetWorthViewModel
    {
        public NetWorthViewModel() =>
            Accounts = new List<AccountViewModel>();

        public decimal NetWorth =>
            Accounts.Where(a => a.IsIncludedInNetWorth).Sum(a => a.CurrentBalance);

        public decimal NetWorthTotal =>
            Accounts.Sum(a => a.CurrentBalance);

        public IEnumerable<AccountViewModel> Accounts { get; set; }
    }
}
