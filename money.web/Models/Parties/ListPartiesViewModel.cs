using money.web.Models.DTO;
using System.Collections.Generic;

namespace money.web.Models
{
    public class ListPartiesViewModel
    {
        public IEnumerable<PartyDTO> Parties { get; set; }
    }
}
