using System.ComponentModel.DataAnnotations;

namespace PawsitiveHealthHub.Models
{
    public class Pets
    {
        [Key]
        public int PetID { get; set; }

        [Required]
        public int OwnerID { get; set; }

        [Required(ErrorMessage = "Please enter your pet's name.")]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "The name can only contain letters and spaces.")]
        [Display(Name = "Name")]
        public string PetName { get; set; }

        [Required(ErrorMessage = "Please enter your pet's species.")]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Species name can only contain letters and spaces.")]
        public string Species { get; set; }

        // Separate fields for age
        [Display(Name = "Years")]
        [Range(0, 35, ErrorMessage = "Please enter a valid age.")]
        public int AgeYears { get; set; } = 0;
        [Display(Name = "Months")]
        [Range(0, 12, ErrorMessage = "Months can only be between 0-12. Otherwise, please use Years.")]
        public int AgeMonths { get; set; } = 0;
        [Display(Name = "Days")]
        [Range(0, 30, ErrorMessage = "Days can only be between 0-30. Otherwise, please use Months.")]
        public int AgeDays { get; set; } = 0;

    }
}
