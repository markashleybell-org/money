using System.Collections.Generic;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class ListEntriesViewModel
    {
        public IEnumerable<Entry> Entries { get; set; }
    }
}
