using System;
using System.ComponentModel.DataAnnotations;

namespace PawsitiveHealthHub.Models
{
    public class CustomDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            DateTime enteredDate;
            bool parsed = DateTime.TryParse(value.ToString(), out enteredDate);

            if (!parsed)
                return false;

            DateTime now = DateTime.Now.Date;
            DateTime max = now.AddMonths(1);

            return enteredDate.Date >= now && enteredDate.Date <= max;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be between today and one month from today.";
        }
    }
}

