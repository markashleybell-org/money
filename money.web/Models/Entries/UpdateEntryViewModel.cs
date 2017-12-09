using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace money.web.Models
{
    public class UpdateEntryViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }
        public int AccountID { get; set; }
        public int? MonthlyBudgetID { get; set; }
        public int? CategoryID { get; set; }
        public int? PartyID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        [StringLength(64)]
        public string Note { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; }
    }
}
