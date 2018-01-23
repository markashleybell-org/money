using System;
using System.Collections.Generic;
using System.Linq;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class ListCategoriesViewModel
    {
        public IEnumerable<IGrouping<string, ListCategoriesCategoryViewModel>> Categories { get; set; }
    }
}
