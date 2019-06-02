using System.Collections.Generic;

namespace money.Models
{
    public class IndexViewModel
    {
        public IEnumerable<AccountViewModel> NetWorthAccounts { get; set; }

        public IEnumerable<AccountViewModel> Accounts { get; set; }
    }
}
