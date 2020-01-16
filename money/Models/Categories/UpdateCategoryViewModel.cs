using System.ComponentModel.DataAnnotations;

namespace Money.Models
{
    public class UpdateCategoryViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter a category name.")]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}
