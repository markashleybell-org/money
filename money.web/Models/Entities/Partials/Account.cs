using money.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace money.web.Models.Entities
{
    public partial class Account
    {
        public Account(int userID, 
            string name, 
            decimal startingBalance, 
            bool isMainAccount, 
            bool isIncludedInNetWorth, 
            int displayOrder)
        {
            UserID = userID;
            Name = name;
            StartingBalance = startingBalance;
            IsMainAccount = isMainAccount;
            IsIncludedInNetWorth = isIncludedInNetWorth;
            DisplayOrder = displayOrder;
        }
    }

    public static class AccountExtensions
    {
        public static Account WithUpdates(this Account account, 
            string name, 
            decimal startingBalance, 
            bool isIncludedInNetWorth)
        {
            return new Account(
                account.ID,
                account.UserID,
                name,
                startingBalance,
                account.IsMainAccount,
                isIncludedInNetWorth,
                account.DisplayOrder
            );
        }
    }
}