using System.Collections.Generic;
using System.Web.Mvc;

namespace money.web.Models
{
    public class CreateCategoryViewModel
    {
        public int AccountID { get; set; }
        public string Name { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; }
    }
}
