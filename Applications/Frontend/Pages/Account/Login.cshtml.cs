using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email обязателен")]
            [EmailAddress(ErrorMessage = "Некорректный формат email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Пароль обязателен")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Запомнить меня")]
            public bool RememberMe { get; set; }
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

            // TODO: Реализовать логику входа
            return RedirectToPage("/Index");
        }
    }
} 