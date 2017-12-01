using money.web.Models.Entities;
using System.Collections.Generic;

namespace money.web.Models
{
    public class ListCategoriesViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
    }
}
