using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.DTO
{
    [d.Table("Categories_MonthlyBudgets")]
    public class Category_MonthlyBudgetDTO
    {
        [d.ExplicitKey]
        public int MonthlyBudgetID { get; set; }
        [d.ExplicitKey]
        public int CategoryID { get; set; }
        public decimal Amount { get; set; }
    }
}
