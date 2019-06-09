using System.Collections.Generic;
using money.Entities;

namespace money.Models
{
    public class ListMonthlyBudgetsViewModel
    {
        public int AccountID { get; set; }

        public IEnumerable<MonthlyBudget> MonthlyBudgets { get; set; }
    }
}
