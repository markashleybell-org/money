using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Money.Models
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Please select an account.")]
        public int? AccountID { get; set; }

        [Required(ErrorMessage = "Please enter a category name.")]
        public string Name { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; }
    }
}
