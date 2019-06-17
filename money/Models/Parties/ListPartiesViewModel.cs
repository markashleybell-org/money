using System.Collections.Generic;
using System.Linq;

namespace Money.Models
{
    public class ListPartiesViewModel
    {
        public IEnumerable<IGrouping<string, ListPartiesPartyViewModel>> Parties { get; set; }
    }
}
