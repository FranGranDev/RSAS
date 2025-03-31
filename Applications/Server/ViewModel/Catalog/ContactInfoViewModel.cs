using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.Catalog
{
    public class ContactInfoViewModel : InputViewModel
    {
        [Required]
        [Display(Name = "ФИО")]
        public string FullName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }
    }
}
