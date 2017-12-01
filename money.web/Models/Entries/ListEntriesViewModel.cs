using money.web.Models.Entities;
using System.Collections.Generic;

namespace money.web.Models
{
    public class ListEntriesViewModel
    {
        public IEnumerable<Entry> Entries { get; set; }
    }
}
