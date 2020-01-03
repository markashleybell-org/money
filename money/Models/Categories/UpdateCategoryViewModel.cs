using System.ComponentModel.DataAnnotations;

namespace Money.Models
{
    public class UpdateCategoryViewModel
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
