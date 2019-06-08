using System.ComponentModel.DataAnnotations;
using money.Entities;

namespace money.Models
{
    public class CreateAccountViewModel
    {
        public string Name { get; set; }

        public AccountType? Type { get; set; } = null;

        public bool IsMainAccount { get; set; }

        public int DisplayOrder { get; set; }

        public decimal StartingBalance { get; set; }

        public bool IncludeInNetWorth { get; set; }
    }
}
