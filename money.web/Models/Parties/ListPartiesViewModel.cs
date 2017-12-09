using System.Collections.Generic;
using money.web.Models.Entities;

namespace money.web.Models
{
    public class ListPartiesViewModel
    {
        public IEnumerable<Party> Parties { get; set; }
    }
}
