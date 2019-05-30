namespace money.Entities
{
    public static class AccountExtensions
    {
        public static Account WithUpdates(
            this Account account,
            string name,
            AccountType type,
            decimal startingBalance,
            int displayOrder,
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
                displayOrder
            );
    }
}
