using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.Entities
{
    [d.Table("Accounts")]
    public partial class Account : Abstract.IEntity
    {
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
    }
}
