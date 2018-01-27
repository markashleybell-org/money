using System.Collections.Generic;
using System.Linq;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class ListPartiesViewModel
    {
        public IEnumerable<IGrouping<string, ListPartiesPartyViewModel>> Parties { get; set; }
    }
}
