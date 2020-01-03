using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Money.Entities;

namespace Money.Models
{
    public class UpdateAccountViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public AccountType Type { get; set; }

        [Required(ErrorMessage = "Please enter a display order.")]
        public int? DisplayOrder { get; set; }

        [Required(ErrorMessage = "Please enter the current account balance.")]
        public decimal? StartingBalance { get; set; }

        public bool IncludeInNetWorth { get; set; }

        public bool IsDormant { get; set; }

        [StringLength(4, MinimumLength = 4, ErrorMessage = "If entered, must be 4 digits.")]
        public string NumberLast4Digits { get; set; }
    }
}
