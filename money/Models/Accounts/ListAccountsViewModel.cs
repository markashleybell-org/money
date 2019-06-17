using System.Collections.Generic;
using Money.Entities;

namespace Money.Models
{
    public class ListAccountsViewModel
    {
        public IEnumerable<Account> Accounts { get; set; }
    }
}
