using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.DTO
{
    [d.Table("Users")]
    public class UserDTO : Abstract.IDTO
    {
        [d.Key]
        public int ID { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
        [StringLength(2048)]
        public string Password { get; set; }
    }
}
