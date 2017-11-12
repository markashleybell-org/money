using money.web.Concrete;

namespace money.web.Models
{
    public class CreateAccountViewModel
    {
        public string Name { get; set; }
        public bool IsMainAccount { get; set; }
        public bool IncludeInNetWorth { get; set; }
        public int DisplayOrder { get; set; }
        public decimal StartingBalance { get; set; }
    }
}
