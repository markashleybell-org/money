using System.ComponentModel.DataAnnotations;

namespace Money.Models
{
    public class UpdatePartyViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter a party name.")]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}
