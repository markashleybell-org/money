using money.web.Models.Entities;
using System.Collections.Generic;

namespace money.web.Models
{
    public class ListPartiesViewModel
    {
        public IEnumerable<Party> Parties { get; set; }
    }
}
