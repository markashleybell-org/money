﻿using System;

namespace money.web.Models.Entities
{
    public partial class Entry
    {
        public Entry(int accountID,
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

        public Entry(int accountID,
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

        public Entry(int accountID,
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
    }

    public static class EntryExtensions
    {
        public static Entry WithUpdates(this Entry entry,
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
