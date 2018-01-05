namespace money.web.Models.Entities
{
    public partial class Account
    {
        public Account(int userID,
            string name,
            AccountType type,
            decimal startingBalance,
            bool isMainAccount,
            bool isIncludedInNetWorth,
            int displayOrder)
        {
            UserID = userID;
            Name = name;
            Type = type;
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
            bool isIncludedInNetWorth) => new Account(
                account.ID,
                account.UserID,
                name,
                account.Type,
                startingBalance,
                account.IsMainAccount,
                isIncludedInNetWorth,
                account.DisplayOrder
            );
    }
}
