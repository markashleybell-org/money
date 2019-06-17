using System.Collections.Generic;
using Money.Entities;

namespace Money.Models
{
    public class ListMonthlyBudgetsViewModel
    {
        public int AccountID { get; set; }

        public IEnumerable<MonthlyBudget> MonthlyBudgets { get; set; }
    }
}
