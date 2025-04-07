using System.ComponentModel.DataAnnotations;

namespace Application.Attributes
{
    public class ValidateDateRange : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dt = (DateTime)value;

            if (dt >= DateTime.Now)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Недопустимая дата доставки");
        }
    }
}