using System.Collections.Generic;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class AccountViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public AccountType Type { get; set; }
        public decimal CurrentBalance { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}
