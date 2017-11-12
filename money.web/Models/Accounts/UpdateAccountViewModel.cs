using System.Web.Mvc;

namespace money.web.Models
{
    public class UpdateAccountViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IncludeInNetWorth { get; set; }
        public decimal StartingBalance { get; set; }
    }
}
