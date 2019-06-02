using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace money.Models
{
    public class CreateEntryViewModel
    {
        [Display(Name = "Account")]
        public int AccountID { get; set; }

        [Display(Name = "Category")]
        public int? CategoryID { get; set; }

        [Display(Name = "Party")]
        public int? PartyID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Transaction Type")]
        public string Type { get; set; }

        public decimal Amount { get; set; }

        [StringLength(64)]
        public string Note { get; set; }

        public IEnumerable<SelectListItem> Types { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public IEnumerable<SelectListItem> Parties { get; set; }

        public IEnumerable<SelectListItem> MonthlyBudgets { get; set; }

        public bool? IsCredit { get; set; }

        public bool ShowCategorySelector { get; set; }

        public decimal Remaining { get; set; }
    }
}
