using money.web.Models.DTO;
using System.Collections.Generic;

namespace money.web.Models
{
    public class ListAccountsViewModel
    {
        public IEnumerable<AccountDTO> Accounts { get; set; }
    }
}
