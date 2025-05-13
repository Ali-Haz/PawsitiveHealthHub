using PawsitiveHealthHub.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace PawsitiveHealthHub.Models
{
    public class Appointments
    {
        [Key]
        public int AppointmentID { get; set; }

        [Required]
        public string OwnerID { get; set; }

        [ForeignKey("OwnerID")]
        public ApplicationUser Owner { get; set; }

        [Required]
        public int PetID { get; set; }

        [ForeignKey("PetID")]
        public Pets Pet { get; set; }

        [Required]
        public string VetID { get; set; }

        [ForeignKey("VetID")]
        public ApplicationUser Vet { get; set; }


        [Required(ErrorMessage = "Please choose a date and time.")]
        [DataType(DataType.DateTime)]
        [CustomDate]
        public DateTime Date { get; set; }
        

        [Required(ErrorMessage = "Please enter the reason.")]
        [StringLength(200)]
        public string Reason { get; set; }

        
    }
}
