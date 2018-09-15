using System;

namespace money.web.Models.Entities
{
    public static class MonthlyBudgetExtensions
    {
        public static MonthlyBudget WithUpdates(
            this MonthlyBudget monthlyBudget,
            DateTime startDate,
            DateTime endDate) => new MonthlyBudget(
                id: monthlyBudget.ID,
                accountId: monthlyBudget.AccountID,
                startDate: startDate,
                endDate: endDate
            );
    }
}
