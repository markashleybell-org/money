using System;
using System.Collections.Generic;
using System.Linq;

namespace Money.Models
{
    public class ListEntriesViewModel
    {
        public IEnumerable<IGrouping<DateTime, ListEntriesEntryViewModel>> Entries { get; set; }
    }
}
