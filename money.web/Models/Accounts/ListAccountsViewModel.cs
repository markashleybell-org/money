using money.web.Models.Entities;
using System.Collections.Generic;

namespace money.web.Models
{
    public class ListAccountsViewModel
    {
        public IEnumerable<Account> Accounts { get; set; }
    }
}
