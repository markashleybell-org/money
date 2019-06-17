using System.Collections.Generic;
using System.Linq;

namespace Money.Models
{
    public class ListCategoriesViewModel
    {
        public IEnumerable<IGrouping<string, ListCategoriesCategoryViewModel>> Categories { get; set; }
    }
}
