using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email обязателен")]
            [EmailAddress(ErrorMessage = "Некорректный формат email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Пароль обязателен")]
            [StringLength(100, ErrorMessage = "Пароль должен быть не менее {2} и не более {1} символов", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Имя обязательно")]
            [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Фамилия обязательна")]
            [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Телефон обязателен")]
            [Phone(ErrorMessage = "Некорректный формат телефона")]
            public string Phone { get; set; }
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Реализовать логику регистрации
            return RedirectToPage("/Account/Login");
        }
    }
} 