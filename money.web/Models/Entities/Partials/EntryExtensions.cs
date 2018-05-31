using System;

namespace money.web.Models.Entities
{
    public static class EntryExtensions
    {
        public static Entry WithUpdates(
            this Entry entry,
            int? categoryID,
            int? partyID,
            DateTime date,
            decimal amount,
            string note) => new Entry(
                id: entry.ID,
                accountID: entry.AccountID,
                monthlyBudgetID: entry.MonthlyBudgetID,
                categoryID: categoryID,
                partyID: partyID,
                date: date,
                amount: amount,
                note: note,
                transferGuid: entry.TransferGUID
            );
    }
}
