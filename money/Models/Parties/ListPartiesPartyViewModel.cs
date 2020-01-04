using Money.Support;

namespace Money.Models
{
    public class ListPartiesPartyViewModel : ISoftDeletableLookupData
    {
        public int ID { get; set; }

        public string Account { get; set; }

        public string Name { get; set; }

        public bool Deleted { get; set; }
    }
}
