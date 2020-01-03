using System.ComponentModel.DataAnnotations;
using Money.Entities;

namespace Money.Models
{
    public class CreateAccountViewModel
    {
        [Required(ErrorMessage = "Please enter an account name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select an account type.")]
        public AccountType? Type { get; set; }

        public bool IsMainAccount { get; set; }

        [Required(ErrorMessage = "Please enter a display order.")]
        public int? DisplayOrder { get; set; } = 0;

        [Required(ErrorMessage = "Please enter the current account balance.")]
        public decimal? StartingBalance { get; set; } = 0.00M;

        public bool IncludeInNetWorth { get; set; }

        [StringLength(4, MinimumLength = 4, ErrorMessage = "If entered, must be 4 digits.")]
        public string NumberLast4Digits { get; set; }
    }
}
