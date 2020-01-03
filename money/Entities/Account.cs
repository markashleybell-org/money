using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace Money.Entities
{
    [d.Table("Accounts")]
    public class Account : IEntity, IAccount
    {
        public Account(
            int userID,
            string name,
            AccountType type,
            decimal startingBalance,
            bool isMainAccount,
            bool isIncludedInNetWorth,
            bool isDormant,
            int displayOrder,
            string numberLast4Digits)
        {
            UserID = userID;
            Name = name;
            Type = type;
            StartingBalance = startingBalance;
            IsMainAccount = isMainAccount;
            IsIncludedInNetWorth = isIncludedInNetWorth;
            IsDormant = isDormant;
            DisplayOrder = displayOrder;
            NumberLast4Digits = numberLast4Digits;
        }

        public Account(
            int id,
            int userId,
            string name,
            AccountType type,
            decimal startingBalance,
            bool isMainAccount,
            bool isIncludedInNetWorth,
            bool isDormant,
            int displayOrder,
            string numberLast4Digits)
        {
            ID = id;
            UserID = userId;
            Name = name;
            Type = type;
            StartingBalance = startingBalance;
            IsMainAccount = isMainAccount;
            IsIncludedInNetWorth = isIncludedInNetWorth;
            IsDormant = isDormant;
            DisplayOrder = displayOrder;
            NumberLast4Digits = numberLast4Digits;
        }

        [d.Key]
        public int ID { get; private set; }

        public int UserID { get; private set; }

        [StringLength(64)]
        public string Name { get; private set; }

        public AccountType Type { get; private set; }

        public decimal StartingBalance { get; private set; }

        public bool IsMainAccount { get; private set; }

        public bool IsIncludedInNetWorth { get; private set; }

        public bool IsDormant { get; private set; }

        public int DisplayOrder { get; private set; }

        public string NumberLast4Digits { get; private set; }

        public Account WithUpdates(
            string name,
            AccountType type,
            decimal startingBalance,
            int displayOrder,
            bool isIncludedInNetWorth,
            bool isDormant,
            string numberLast4Digits) => new Account(
                ID,
                UserID,
                name,
                type,
                startingBalance,
                IsMainAccount,
                isIncludedInNetWorth,
                isDormant,
                displayOrder,
                numberLast4Digits
            );
    }
}
