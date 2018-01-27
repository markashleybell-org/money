using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace money.web.Models
{
    public class CreateCategoryViewModel
    {
        [Display(Name = "Account")]
        public int AccountID { get; set; }
        public string Name { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; }
    }
}
