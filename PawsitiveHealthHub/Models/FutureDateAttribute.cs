using System.ComponentModel.DataAnnotations;

namespace PawsitiveHealthHub.Models
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime enteredDate = Convert.ToDateTime(value);
            if (enteredDate < DateTime.Today)
            {
                return new ValidationResult(ErrorMessage ?? "Date must be today or in the future.");
            }
            return ValidationResult.Success;
        }
    }
}