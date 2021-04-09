using System;
using d = Dapper.Contrib.Extensions;

namespace Money.Entities
{
    [d.Table("MonthlyBudgets")]
    public class MonthlyBudget : IEntity
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

        public MonthlyBudget(
            int id,
            int accountId,
            DateTime startDate,
            DateTime endDate)
        {
            ID = id;
            AccountID = accountId;
            StartDate = startDate;
            EndDate = endDate;
        }

        [d.Key]
        public int ID { get; private set; }

        public int AccountID { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public MonthlyBudget WithUpdates(
            DateTime startDate,
            DateTime endDate) =>
            new(
                id: ID,
                accountId: AccountID,
                startDate: startDate,
                endDate: endDate
            );
    }
}
