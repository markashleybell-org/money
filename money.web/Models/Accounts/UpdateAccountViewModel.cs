using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class UpdateAccountViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }

        public string Name { get; set; }

        public AccountType Type { get; set; }

        [Display(Name = "Display order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Starting balance")]
        public decimal StartingBalance { get; set; }

        [Display(Name = "Include balance in net worth")]
        public bool IncludeInNetWorth { get; set; }

        [Display(Name = "Account is dormant")]
        public bool IsDormant { get; set; }
    }
}
