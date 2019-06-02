using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.Entities
{
    [d.Table("Parties")]
    public partial class Party : IEntity
    {
        public Party(int accountID, string name)
        {
            AccountID = accountID;
            Name = name;
        }

        public Party(
            int id,
            int accountId,
            string name)
        {
            ID = id;
            AccountID = accountId;
            Name = name;
        }

        [d.Key]
        public int ID { get; private set; }

        public int AccountID { get; private set; }

        [StringLength(64)]
        public string Name { get; private set; }

        public Party WithUpdates(
            string name)
            => new Party(
                id: ID,
                accountId: AccountID,
                name: name
            );
    }
}
