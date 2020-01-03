using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace Money.Entities
{
    [d.Table("Categories")]
    public class Category : IEntity
    {
        public Category(int accountID, string name)
        {
            AccountID = accountID;
            Name = name;
        }

        public Category(
            int id,
            int accountId,
            string name,
            int displayOrder)
        {
            ID = id;
            AccountID = accountId;
            Name = name;
            DisplayOrder = displayOrder;
        }

        [d.Key]
        public int ID { get; private set; }

        public int AccountID { get; private set; }

        [StringLength(64)]
        public string Name { get; private set; }

        public int DisplayOrder { get; private set; }

        public Category WithUpdates(string name) =>
            new Category(
                ID,
                AccountID,
                name,
                DisplayOrder
            );
    }
}
