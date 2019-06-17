using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Money.Models
{
    public class CreateCategoryViewModel
    {
        public int AccountID { get; set; }

        public string Name { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; }
    }
}
