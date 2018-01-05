using System.Collections.Generic;

namespace money.web.Models
{
    public class IndexViewModel
    {
        public IEnumerable<AccountViewModel> Accounts { get; set; }
    }
}
