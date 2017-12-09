using System.Collections.Generic;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class ListCategoriesViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
    }
}
