using System.Collections.Generic;

namespace money.web.Models
{
    public class IndexViewModel
    {
        public int? AccountID { get; set; }
        public IEnumerable<AccountViewModel> Accounts { get; set; }
    }
}
