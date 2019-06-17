using System.Collections.Generic;
using Money.Entities;

namespace Money.Models
{
    public class AccountViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public AccountType Type { get; set; }

        public decimal CurrentBalance { get; set; }

        public bool IsIncludedInNetWorth { get; set; }

        public bool IsDormant { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }

        public int? UpdatedCategoryID { get; set; }
    }
}
