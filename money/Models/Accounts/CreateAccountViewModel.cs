﻿using System.ComponentModel.DataAnnotations;
using money.Entities;

namespace money.Models
{
    public class CreateAccountViewModel
    {
        public string Name { get; set; }

        public AccountType? Type { get; set; } = null;

        public bool IsMainAccount { get; set; }

        [Display(Name = "Display order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Starting balance")]
        public decimal StartingBalance { get; set; }

        [Display(Name = "Include balance in net worth")]
        public bool IncludeInNetWorth { get; set; }
    }
}
