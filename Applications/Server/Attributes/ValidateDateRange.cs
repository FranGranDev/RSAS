using System.ComponentModel.DataAnnotations;

namespace Application.Attributes
{
    public class ValidateDateRange : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dt = (DateTime)value;

            if (dt >= DateTime.Now)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Недопустимая дата доставки");
            }
        }
    }
}
