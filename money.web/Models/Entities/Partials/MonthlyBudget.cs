using System;

namespace money.web.Models.Entities
{
    public partial class MonthlyBudget
    {
        public MonthlyBudget(
            int accountID,
            DateTime startDate,
            DateTime endDate)
        {
            AccountID = accountID;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
