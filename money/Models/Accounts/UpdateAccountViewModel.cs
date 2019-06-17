using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Money.Entities;

namespace Money.Models
{
    public class UpdateAccountViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        public string Name { get; set; }

        public AccountType Type { get; set; }

        public int DisplayOrder { get; set; }

        public decimal StartingBalance { get; set; }

        public bool IncludeInNetWorth { get; set; }

        public bool IsDormant { get; set; }
    }
}
