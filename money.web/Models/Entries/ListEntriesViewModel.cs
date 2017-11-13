using money.web.Models.DTO;
using System.Collections.Generic;

namespace money.web.Models
{
    public class ListEntriesViewModel
    {
        public IEnumerable<EntryDTO> Entries { get; set; }
    }
}
