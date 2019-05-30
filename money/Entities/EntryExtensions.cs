using System;

namespace money.Entities
{
    public static class EntryExtensions
    {
        public static Entry WithUpdates(
            this Entry entry,
            int? categoryID,
            int? partyID,
            DateTime date,
            decimal amount,
            string note)
            => new Entry(
                id: entry.ID,
                accountId: entry.AccountID,
                monthlyBudgetId: entry.MonthlyBudgetID,
                categoryId: categoryID,
                partyId: partyID,
                date: date,
                amount: amount,
                note: note,
                transferGuid: entry.TransferGUID
            );
    }
}
