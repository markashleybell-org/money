using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace money.web.Models
{
    public class UpdateMonthlyBudgetViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int AccountID { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Start budget on")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "End budget on")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public IEnumerable<MonthlyBudgetCategoryViewModel> Categories { get; set; }
    }
}
