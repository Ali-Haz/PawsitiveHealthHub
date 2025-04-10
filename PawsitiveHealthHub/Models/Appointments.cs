using System.ComponentModel.DataAnnotations;

namespace PawsitiveHealthHub.Models
{
    public class Appointments
    {
        [Key]
        public int AppointmentID { get; set; }

        [Required]
        public int OwnerID { get; set; }

        [Required]
        public int PetID { get; set; }

        [Required]
        public int VetID { get; set; }

        private DateTime _date;

        [Required(ErrorMessage = "Please choose a date and time.")]
        [FutureDate(ErrorMessage = "Date must be today or in the future.")]
        [DataType(DataType.DateTime)]
        [CustomValidation(typeof(Appointments), nameof(ValidateAppointmentTime))]
        public DateTime Date
        {
            get => _date;
            set => _date = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
        }

        public static ValidationResult ValidateAppointmentTime(DateTime date, ValidationContext context)
        {
            var time = date.TimeOfDay;
            if (time < TimeSpan.FromHours(8) || time > TimeSpan.FromHours(17))
            {
                return new ValidationResult("Appointments are only available from 8 AM to 5 PM.");
            }
            return ValidationResult.Success;
        }


        [Required(ErrorMessage = "Please enter the reason.")]
        [StringLength(200)]
        public string Reason { get; set; }


    }
}
