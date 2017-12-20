using System;
using System.Collections.Generic;
using System.Linq;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class ListEntriesViewModel
    {
        public IEnumerable<IGrouping<DateTime, ListEntriesEntryViewModel>> Entries { get; set; }
    }
}
