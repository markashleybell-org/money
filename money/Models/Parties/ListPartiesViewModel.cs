using System.Collections.Generic;
using System.Linq;

namespace Money.Models
{
    public class ListPartiesViewModel
    {
        public IEnumerable<IGrouping<(int AccountID, string AccountName), ListPartiesPartyViewModel>> Parties { get; set; }
    }
}
