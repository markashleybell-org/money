using Money.Support;

namespace Money.Models
{
    public class ListCategoriesCategoryViewModel : ISoftDeletableLookupData
    {
        public int ID { get; set; }

        public int AccountID { get; set; }

        public string Account { get; set; }

        public string Name { get; set; }

        public bool Deleted { get; set; }
    }
}
