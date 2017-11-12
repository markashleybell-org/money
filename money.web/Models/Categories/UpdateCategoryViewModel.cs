using money.web.Concrete;
using money.web.Models.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace money.web.Models
{
    public class UpdateCategoryViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }
        public int AccountID { get; set; }
        public string Name { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; }
    }
}
