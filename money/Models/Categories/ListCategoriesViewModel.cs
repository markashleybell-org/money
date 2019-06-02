using System.Collections.Generic;
using System.Linq;

namespace money.Models
{
    public class ListCategoriesViewModel
    {
        public IEnumerable<IGrouping<string, ListCategoriesCategoryViewModel>> Categories { get; set; }
    }
}
