using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace money.web.Models.Entities
{
    public enum AccountType
    {
        [Display(Name = "Current Accounts")]
        Current,
        [Display(Name = "Savings Accounts")]
        Savings,
        [Display(Name = "Credit Cards")]
        CreditCard,
        [Display(Name = "Loans")]
        Mortgage,
        [Display(Name = "Pensions")]
        Pension
    }
}
