using System.Collections.Generic;

namespace money.Models
{
    public class IndexViewModel
    {
        public IEnumerable<AccountViewModel> Accounts { get; set; }

        public NetWorthViewModel NetWorthViewModel { get; set; }
    }
}
