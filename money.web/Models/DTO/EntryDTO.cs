using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.DTO
{
    [d.Table("Entries")]
    public class EntryDTO : Abstract.IDTO
    {
        [d.Key]
        public int ID { get; set; }
        public int AccountID { get; set; }
        public int? MonthlyBudgetID { get; set; }
        public int? CategoryID { get; set; }
        public int PartyID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        [StringLength(64)]
        public string Note { get; set; }
        public Guid? TransferGUID { get; set; }
    }
}
