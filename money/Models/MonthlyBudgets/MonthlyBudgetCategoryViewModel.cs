using Microsoft.AspNetCore.Mvc;

namespace money.Models
{
    public class MonthlyBudgetCategoryViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int CategoryID { get; set; }

        public string Name { get; set; }

        public decimal Amount { get; set; }
    }
}