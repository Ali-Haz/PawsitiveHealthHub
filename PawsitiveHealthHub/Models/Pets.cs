using PawsitiveHealthHub.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PawsitiveHealthHub.Models
{
    public class Pets
    {
        [Key]
        public int PetID { get; set; }

        [Required]
        public string OwnerID { get; set; }

        [ForeignKey("OwnerID")]
        public ApplicationUser Owner { get; set; }

        [Required(ErrorMessage = "Please enter your pet's name.")]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "The name can only contain letters and spaces.")]
        [Display(Name = "Name")]
        public string PetName { get; set; }

        [Required(ErrorMessage = "Please enter your pet's species.")]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Species name can only contain letters and spaces.")]
        public string Species { get; set; }

        [Display(Name = "Years")]
        [Range(0, 35)]
        public int AgeYears { get; set; } = 0;

        [Display(Name = "Months")]
        [Range(0, 12)]
        public int AgeMonths { get; set; } = 0;

        [Display(Name = "Days")]
        [Range(0, 30)]
        public int AgeDays { get; set; } = 0;

        // Navigation Properties
        public ICollection<MedRecords> MedRecords { get; set; }
        public ICollection<Appointments> Appointments { get; set; }
    }
}
