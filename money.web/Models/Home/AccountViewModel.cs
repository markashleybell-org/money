using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace money.web.Models
{
    public class AccountViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal CurrentBalance { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}