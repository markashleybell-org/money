using System.Collections.Generic;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class ListAccountsViewModel
    {
        public IEnumerable<Account> Accounts { get; set; }
    }
}
