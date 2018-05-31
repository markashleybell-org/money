using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.Entities
{
    [d.Table("Parties")]
    public partial class Party : Abstract.IEntity
    {
        public Party(
            int id,
            int accountID,
            string name)
        {
            ID = id;
            AccountID = accountID;
            Name = name;
        }

        [d.Key]
        public int ID { get; private set; }

        public int AccountID { get; private set; }

        [StringLength(64)]
        public string Name { get; private set; }
    }
}
