using System.Collections.Generic;
using System.Linq;

namespace Money.Models
{
    public class ListCategoriesViewModel
    {
        public IEnumerable<IGrouping<(int AccountID, string AccountName), ListCategoriesCategoryViewModel>> Categories { get; set; }
    }
}
