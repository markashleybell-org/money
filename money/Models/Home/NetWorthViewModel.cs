using System.Collections.Generic;
using System.Linq;
using Money.Entities;

namespace Money.Models
{
    public class NetWorthViewModel
    {
        public NetWorthViewModel() =>
            Accounts = new List<AccountViewModel>();

        public decimal NetWorth =>
            Accounts.Where(a => a.IsIncludedInNetWorth).Sum(a => a.CurrentBalance);

        public decimal NetWorthTotal =>
            Accounts.Sum(a => a.CurrentBalance);

        public decimal SavingsTotal =>
            Accounts.Where(a => a.IsIncludedInNetWorth && (a.Type == AccountType.Savings || a.Type == AccountType.Investment)).Sum(a => a.CurrentBalance);

        public IEnumerable<AccountViewModel> Accounts { get; set; }
    }
}
