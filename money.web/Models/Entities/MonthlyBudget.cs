using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.Entities
{
    [d.Table("MonthlyBudgets")]
    public partial class MonthlyBudget : Abstract.IEntity
    {
        public MonthlyBudget(int id, int accountID, DateTime startDate, DateTime endDate)
        {
            ID = id;
            AccountID = accountID;
            StartDate = startDate;
            EndDate = endDate;
        }

        [d.Key]
        public int ID { get; private set; }
        public int AccountID { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
    }
}
