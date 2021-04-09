using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace Money.Entities
{
    [d.Table("Parties")]
    public class Party : IEntity, ISoftDeletable<Party>
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

        private Party(
            int id,
            int accountId,
            string name,
            bool deleted)
        {
            ID = id;
            AccountID = accountId;
            Name = name;
            Deleted = deleted;
        }

        [d.Key]
        public int ID { get; private set; }

        public int AccountID { get; private set; }

        [StringLength(64)]
        public string Name { get; private set; }

        public bool Deleted { get; private set; }

        public Party WithUpdates(
            string name) =>
            new(
                id: ID,
                accountId: AccountID,
                name: name
            );

        public Party ForDeletion() =>
            new(
                ID,
                AccountID,
                Name,
                deleted: true
            );

        public Party ForUndeletion() =>
            new(
                ID,
                AccountID,
                Name,
                deleted: false
            );
    }
}
