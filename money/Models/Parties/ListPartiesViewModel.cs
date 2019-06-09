using System.Collections.Generic;
using System.Linq;

namespace money.Models
{
    public class ListPartiesViewModel
    {
        public IEnumerable<IGrouping<string, ListPartiesPartyViewModel>> Parties { get; set; }
    }
}
