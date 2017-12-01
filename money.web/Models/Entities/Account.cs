using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.Entities
{
    [d.Table("Accounts")]
    public partial class Account : Abstract.IEntity
    {
        public Account(int id, int userID, string name, decimal startingBalance, bool isMainAccount, bool isIncludedInNetWorth, int displayOrder)
        {
            ID = id;
            UserID = userID;
            Name = name;
            StartingBalance = startingBalance;
            IsMainAccount = isMainAccount;
            IsIncludedInNetWorth = isIncludedInNetWorth;
            DisplayOrder = displayOrder;
        }

        [d.Key]
        public int ID { get; private set; }
        public int UserID { get; private set; }
        [StringLength(64)]
        public string Name { get; private set; }
        public decimal StartingBalance { get; private set; }
        public bool IsMainAccount { get; private set; }
        public bool IsIncludedInNetWorth { get; private set; }
        public int DisplayOrder { get; private set; }
    }
}
