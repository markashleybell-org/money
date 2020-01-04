using System.Collections.Generic;
using System.Linq;

namespace Money.Models
{
    public class ListPartiesViewModel
    {
        public IEnumerable<IGrouping<(int accountID, string accountName), ListPartiesPartyViewModel>> Parties { get; set; }
    }
}
