using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.Users
{
    public class PasswordViewModel : InputViewModel
    {
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        [StringLength(100, ErrorMessage = "{0} должен быть как минимум {2} и максимум {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпали")]
        public string ConfirmPassword { get; set; }
    }
}
