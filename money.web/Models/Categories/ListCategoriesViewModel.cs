using money.web.Models.DTO;
using System.Collections.Generic;

namespace money.web.Models
{
    public class ListCategoriesViewModel
    {
        public IEnumerable<CategoryDTO> Categories { get; set; }
    }
}
