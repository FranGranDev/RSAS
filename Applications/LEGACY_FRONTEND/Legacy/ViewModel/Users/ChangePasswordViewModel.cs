using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.Users
{
    public class ChangePasswordViewModel : InputViewModel
    {
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        [StringLength(100, ErrorMessage = "{0} должен быть как минимум {2} и максимум {1} символов.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпали.")]
        public string ConfirmPassword { get; set; }
    }
}