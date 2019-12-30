using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace Money.Entities
{
    [d.Table("Accounts")]
    public class Account : IEntity
    {
        public Account(
            int userID,
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

        public Account(
            int id,
            int userId,
            string name,
            AccountType type,
            decimal startingBalance,
            bool isMainAccount,
            bool isIncludedInNetWorth,
            bool isDormant,
            int displayOrder)
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
        }

        [d.Key]
        public int ID { get; }

        public int UserID { get; }

        [StringLength(64)]
        public string Name { get; }

        public AccountType Type { get; }

        public decimal StartingBalance { get; }

        public bool IsMainAccount { get; }

        public bool IsIncludedInNetWorth { get; }

        public bool IsDormant { get; }

        public int DisplayOrder { get; }

        public Account WithUpdates(
            string name,
            AccountType type,
            decimal startingBalance,
            int displayOrder,
            bool isIncludedInNetWorth,
            bool isDormant) => new Account(
                ID,
                UserID,
                name,
                type,
                startingBalance,
                IsMainAccount,
                isIncludedInNetWorth,
                isDormant,
                displayOrder
            );
    }
}
