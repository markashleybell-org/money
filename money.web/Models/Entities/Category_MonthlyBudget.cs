using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.Entities
{
    [d.Table("Categories_MonthlyBudgets")]
    public partial class Category_MonthlyBudget
    {
        public Category_MonthlyBudget(int monthlyBudgetID, int categoryID, decimal amount)
        {
            MonthlyBudgetID = monthlyBudgetID;
            CategoryID = categoryID;
            Amount = amount;
        }

        [d.ExplicitKey]
        public int MonthlyBudgetID { get; private set; }
        [d.ExplicitKey]
        public int CategoryID { get; private set; }
        public decimal Amount { get; private set; }
    }
}
