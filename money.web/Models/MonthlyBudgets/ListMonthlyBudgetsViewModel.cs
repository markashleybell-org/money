using money.web.Models.Entities;
using System.Collections.Generic;

namespace money.web.Models
{
    public class ListMonthlyBudgetsViewModel
    {
        public int AccountID { get; set; }

        public IEnumerable<MonthlyBudget> MonthlyBudgets { get; set; }
    }
}
