using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.Entities
{
    [d.Table("Entries")]
    public partial class Entry : Abstract.IEntity
    {
        public Entry(
            int id,
            int accountID,
            int? monthlyBudgetID,
            int? categoryID,
            int? partyID,
            DateTime date,
            decimal amount,
            string note,
            Guid? transferGuid)
        {
            ID = id;
            AccountID = accountID;
            MonthlyBudgetID = monthlyBudgetID;
            CategoryID = categoryID;
            PartyID = partyID;
            Date = date;
            Amount = amount;
            Note = note;
            TransferGUID = transferGuid;
        }

        [d.Key]
        public int ID { get; private set; }

        public int AccountID { get; private set; }

        public int? MonthlyBudgetID { get; private set; }

        public int? CategoryID { get; private set; }

        public int? PartyID { get; private set; }

        public DateTime Date { get; private set; }

        public decimal Amount { get; private set; }

        [StringLength(64)]
        public string Note { get; private set; }

        public Guid? TransferGUID { get; private set; }
    }
}
