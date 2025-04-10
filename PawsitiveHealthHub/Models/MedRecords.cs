using System.ComponentModel.DataAnnotations;

namespace PawsitiveHealthHub.Models
{
    public class MedRecords
    {
        [Key]
        public int RecordID { get; set; }

        [Required]
        public int PetID { get; set; } // Foreign key linking to Pet

        [Required]
        public int VetID { get; set; } // Foreign key linking to User (Vet)

        [Required(ErrorMessage = "Please enter the diagnosis.")]
        [StringLength(150)]
        public string Diagnosis { get; set; }

        [Required(ErrorMessage = "Please enter the treatment.")]
        [StringLength(150)]
        public string Treatment { get; set; }

    }
}
