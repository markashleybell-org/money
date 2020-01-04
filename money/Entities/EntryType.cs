using System;
using System.ComponentModel.DataAnnotations;

namespace Money.Entities
{
    [Flags]
    public enum EntryType
    {
        Unknown = 0,

        [Display(Description = "Payment to an external payee")]
        Debit = 1,

        [Display(Description = "Money received or paid in")]
        Credit = 2,

        Transfer = 4
    }
}
