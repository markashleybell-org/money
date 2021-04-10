using System.ComponentModel.DataAnnotations;

namespace Money.Entities
{
    public enum AccountType
    {
        [Display(Name = "Current Accounts")]
        Current = 10,

        [Display(Name = "Savings Accounts")]
        Savings = 20,

        [Display(Name = "Investments")]
        Investment = 30,

        [Display(Name = "Credit Cards")]
        CreditCard = 40,

        [Display(Name = "Loans")]
        Loans = 50
    }
}
