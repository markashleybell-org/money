using System.Collections.Generic;

namespace Money.Models
{
    public class IndexViewModel
    {
        public IEnumerable<AccountViewModel> Accounts { get; set; }

        public NetWorthViewModel NetWorthViewModel { get; set; }
    }
}
