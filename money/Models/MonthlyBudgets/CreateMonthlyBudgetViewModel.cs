using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Money.Models
{
    public class CreateMonthlyBudgetViewModel
    {
        public int AccountID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public IEnumerable<MonthlyBudgetCategoryViewModel> Categories { get; set; }
    }
}
