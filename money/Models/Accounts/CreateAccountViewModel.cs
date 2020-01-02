using System.ComponentModel.DataAnnotations;
using Money.Entities;

namespace Money.Models
{
    public class CreateAccountViewModel
    {
        public string Name { get; set; }

        public AccountType? Type { get; set; } = null;

        public bool IsMainAccount { get; set; }

        public int DisplayOrder { get; set; }

        public decimal StartingBalance { get; set; }

        public bool IncludeInNetWorth { get; set; }

        [StringLength(4, MinimumLength = 4, ErrorMessage = "If entered, must be 4 digits")]
        public string NumberLast4Digits { get; set; }
    }
}
