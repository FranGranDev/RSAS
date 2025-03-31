using Application.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.Users
{
    public class ClientViewModel : InputViewModel
    {
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        [EmailAddress]
        [Display(Name = "Эл.почта")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }
    }
}
