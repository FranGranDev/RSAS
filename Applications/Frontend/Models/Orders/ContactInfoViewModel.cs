using System.ComponentModel.DataAnnotations;

namespace Frontend.Models.Orders
{
    public class ContactInfoViewModel
    {
        public bool Editable { get; set; } = true;

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Имя обязательно для заполнения")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; }

        [Display(Name = "Телефон")]
        [Required(ErrorMessage = "Телефон обязателен для заполнения")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string Phone { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
} 