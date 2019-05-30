using System.ComponentModel.DataAnnotations;

namespace money.Entities
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
