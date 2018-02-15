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
            bool isDormant,
            int displayOrder)
        {
            UserID = userID;
            Name = name;
            Type = type;
            StartingBalance = startingBalance;
            IsMainAccount = isMainAccount;
            IsIncludedInNetWorth = isIncludedInNetWorth;
            IsDormant = isDormant;
            DisplayOrder = displayOrder;
        }
    }

    public static class AccountExtensions
    {
        public static Account WithUpdates(this Account account,
            string name,
            AccountType type,
            decimal startingBalance,
            bool isIncludedInNetWorth,
            bool isDormant) => new Account(
                account.ID,
                account.UserID,
                name,
                type,
                startingBalance,
                account.IsMainAccount,
                isIncludedInNetWorth,
                isDormant,
                account.DisplayOrder
            );
    }
}
