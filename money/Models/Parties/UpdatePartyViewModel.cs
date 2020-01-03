using System.ComponentModel.DataAnnotations;

namespace Money.Models
{
    public class UpdatePartyViewModel
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
