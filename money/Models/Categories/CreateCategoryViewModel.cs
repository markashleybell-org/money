using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace money.Models
{
    public class CreateCategoryViewModel
    {
        public int AccountID { get; set; }

        public string Name { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; }
    }
}
