namespace money.web.Models.Entities
{
    public static class AccountExtensions
    {
        public static Account WithUpdates(
            this Account account,
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
