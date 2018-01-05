using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace money.web.Models.Entities
{
    public enum AccountType
    {
        [Display(Name = "Current Account")]
        Current,
        [Display(Name = "Savings Account")]
        Savings,
        [Display(Name = "Credit Card")]
        CreditCard,
        [Display(Name = "Loan")]
        Loan,
        [Display(Name = "Mortgage")]
        Mortgage
    }
}
