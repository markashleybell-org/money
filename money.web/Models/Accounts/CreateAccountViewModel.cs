using System.ComponentModel.DataAnnotations;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class CreateAccountViewModel
    {
        public string Name { get; set; }
        public AccountType? Type { get; set; } = null;
        public bool IsMainAccount { get; set; }
        public bool IncludeInNetWorth { get; set; }
        public int DisplayOrder { get; set; }
        [Display(Name = "Starting balance")]
        public decimal StartingBalance { get; set; }
    }
}
