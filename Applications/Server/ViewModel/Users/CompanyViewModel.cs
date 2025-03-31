using Application.Areas.Identity.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.Users
{
    public class CompanyViewModel : InputViewModel
    {
        [Display(Name = "Название компании")]
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        public string Name { get; set; }

        [Display(Name = "ИНН компании")]
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        public string Inn { get; set; }

        [Display(Name = "КПП компании")]
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        public string Kpp { get; set; }

        [Display(Name = "Название банка")]
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        public string BankName { get; set; }

        [Display(Name = "БИК банка")]
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        public string BankBic { get; set; }

        [Display(Name = "Расчетный счет компании")]
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        public string BankAccount { get; set; }

        [Display(Name = "Эл.почта компании")]
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Телефон компании")]
        [Required(ErrorMessage = "Необходимо заполнить поле {0}")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }


        public CompanyViewModel()
        {

        }
        public CompanyViewModel(Company company)
        {
            Name = company.Name;
            Inn = company.Inn;
            Kpp = company.Kpp;
            BankName = company.BankName;
            BankBic = company.BankBic;
            BankAccount = company.BankAccount;
            Phone = company.Phone;
            Email = company.Email;
        }
        public CompanyViewModel(IConfiguration configuration)
        {
            Name = configuration["CompanyDetails:Name"];
            Inn = configuration["CompanyDetails:Inn"];
            Kpp = configuration["CompanyDetails:Kpp"];
            BankName = configuration["CompanyDetails:BankName"];
            BankBic = configuration["CompanyDetails:BankBic"];
            BankAccount = configuration["CompanyDetails:BankAccount"];
            Phone = configuration["CompanyDetails:Phone"];
            Email = configuration["CompanyDetails:Email"];
        }
    }
}
