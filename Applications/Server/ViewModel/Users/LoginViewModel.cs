using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.Users
{
    public class LoginViewModel : InputViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        [Display(Name = "Эл.почта")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня?")] public bool RememberMe { get; set; }
    }
}