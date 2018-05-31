using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.Entities
{
    [d.Table("Categories")]
    public partial class Category : Abstract.IEntity
    {
        public Category(int id, int accountID, string name, int displayOrder)
        {
            ID = id;
            AccountID = accountID;
            Name = name;
            DisplayOrder = displayOrder;
        }

        [d.Key]
        public int ID { get; private set; }

        public int AccountID { get; private set; }

        [StringLength(64)]
        public string Name { get; private set; }

        public int DisplayOrder { get; private set; }
    }
}
