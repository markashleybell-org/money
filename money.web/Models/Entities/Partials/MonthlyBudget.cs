using System;

namespace money.web.Models.Entities
{
    public partial class MonthlyBudget
    {
        public MonthlyBudget(int accountID,
            DateTime startDate,
            DateTime endDate)
        {
            AccountID = accountID;
            StartDate = startDate;
            EndDate = endDate;
        }
    }

    public static class MonthlyBudgetExtensions
    {
        public static MonthlyBudget WithUpdates(this MonthlyBudget monthlyBudget,
            DateTime startDate,
            DateTime endDate) => new MonthlyBudget(
                id: monthlyBudget.ID,
                accountID: monthlyBudget.AccountID,
                startDate: startDate,
                endDate: endDate
            );
    }
}
