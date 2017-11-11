using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.DTO
{
    [d.Table("MonthlyBudgets")]
    public class MonthlyBudgetDTO : Abstract.IDTO
    {
        [d.Key]
        public int ID { get; set; }
        public int AccountID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
