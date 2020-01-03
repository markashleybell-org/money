using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Money.Models
{
    public class CreateEntryViewModel
    {
        public int AccountID { get; set; }

        public int? CategoryID { get; set; }

        public int? PartyID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTime.Now;

        public string Type { get; set; }

        [Required(ErrorMessage = "Please enter an amount.")]
        public decimal? Amount { get; set; }

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
