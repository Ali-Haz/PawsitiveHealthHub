using PawsitiveHealthHub.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace PawsitiveHealthHub.Models
{
    public class MedRecords
    {
        [Key]
        public int RecordID { get; set; }

        [Required]
        public int PetID { get; set; }

        [ForeignKey("PetID")]
        public Pets Pet { get; set; }

        [Required]
        public string VetID { get; set; }

        [ForeignKey("VetID")]
        public ApplicationUser Vet { get; set; }

        [Required(ErrorMessage = "Please enter the diagnosis.")]
        [StringLength(150)]
        public string Diagnosis { get; set; }

        [Required(ErrorMessage = "Please enter the treatment.")]
        [StringLength(150)]
        public string Treatment { get; set; }
    }
}
