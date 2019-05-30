using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.Entities
{
    [d.Table("Entries")]
    public partial class Entry : IEntity
    {
        public Entry(
            int accountID,
            int? monthlyBudgetID,
            int? categoryID,
            int? partyID,
            DateTime date,
            decimal amount,
            string note)
        {
            AccountID = accountID;
            MonthlyBudgetID = monthlyBudgetID;
            CategoryID = categoryID;
            PartyID = partyID;
            Date = date;
            Amount = amount;
            Note = note;
        }

        public Entry(
            int accountID,
            int? monthlyBudgetID,
            int? categoryID,
            DateTime date,
            decimal amount,
            string note,
            Guid? transferGuid)
        {
            AccountID = accountID;
            MonthlyBudgetID = monthlyBudgetID;
            CategoryID = categoryID;
            Date = date;
            Amount = amount;
            Note = note;
            TransferGUID = transferGuid;
        }

        public Entry(
            int accountID,
            int? monthlyBudgetID,
            DateTime date,
            decimal amount,
            string note,
            Guid? transferGuid)
        {
            AccountID = accountID;
            MonthlyBudgetID = monthlyBudgetID;
            Date = date;
            Amount = amount;
            Note = note;
            TransferGUID = transferGuid;
        }

        public Entry(
            int id,
            int accountId,
            int? monthlyBudgetId,
            int? categoryId,
            int? partyId,
            DateTime date,
            decimal amount,
            string note,
            Guid? transferGuid)
        {
            ID = id;
            AccountID = accountId;
            MonthlyBudgetID = monthlyBudgetId;
            CategoryID = categoryId;
            PartyID = partyId;
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
