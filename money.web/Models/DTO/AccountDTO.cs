using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.DTO
{
    [d.Table("Accounts")]
    public class AccountDTO : Abstract.IDTO
    {
        [d.Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        [StringLength(64)]
        public string Name { get; set; }
        public int Type { get; set; }
        public bool IsMainAccount { get; set; }
        public bool IsIncludedInNetWorth { get; set; }
        public int DisplayOrder { get; set; }
        public decimal StartingBalance { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
