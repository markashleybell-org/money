using System.Collections.Generic;
using money.Entities;

namespace money.Models
{
    public class ListAccountsViewModel
    {
        public IEnumerable<Account> Accounts { get; set; }
    }
}
