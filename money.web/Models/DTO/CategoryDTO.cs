using System;
using System.ComponentModel.DataAnnotations;
using d = Dapper.Contrib.Extensions;

namespace money.web.Models.DTO
{
    [d.Table("Categories")]
    public class CategoryDTO : Abstract.IDTO
    {
        [d.Key]
        public int ID { get; set; }
        public int AccountID { get; set; }
        [StringLength(64)]
        public string Name { get; set; }
        public int CategoryType { get; set; }
        public int? ExpenseType { get; set; }
    }
}
