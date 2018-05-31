using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.Entities
{
    [d.Table("Users")]
    public partial class User : Abstract.IEntity
    {
        public User(
            int id,
            string email,
            string password)
        {
            ID = id;
            Email = email;
            Password = password;
        }

        [d.Key]
        public int ID { get; private set; }

        [StringLength(256)]
        public string Email { get; private set; }

        [StringLength(2048)]
        public string Password { get; private set; }
    }
}
