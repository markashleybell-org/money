using System.Collections.Generic;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class ListMonthlyBudgetsViewModel
    {
        public int AccountID { get; set; }

        public IEnumerable<MonthlyBudget> MonthlyBudgets { get; set; }
    }
}
