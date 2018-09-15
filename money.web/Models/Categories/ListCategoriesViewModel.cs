using System.Collections.Generic;
using System.Linq;

namespace money.web.Models
{
    public class ListCategoriesViewModel
    {
        public IEnumerable<IGrouping<string, ListCategoriesCategoryViewModel>> Categories { get; set; }
    }
}
