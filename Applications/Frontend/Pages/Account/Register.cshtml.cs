using Application.DTOs;
using Frontend.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace Frontend.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IAuthService _authService;

    public RegisterModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public RegisterDto Input { get; set; }

    public string ReturnUrl { get; set; }

    public void OnGet(string returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            try
            {
                var result = await _authService.RegisterAsync(Input);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Регистрация успешно завершена!";
                    return RedirectToPage("/Account/Profile");
                }

                // Обработка ошибок от сервера
                var errorMessage = result.Message;
                    
                // Проверяем наличие ошибок о занятом имени пользователя или email
                if (errorMessage.Contains("Username") && errorMessage.Contains("is already taken"))
                {
                    ModelState.AddModelError("Input.UserName", "Это имя пользователя уже занято");
                }
                if (errorMessage.Contains("Email") && errorMessage.Contains("is already taken"))
                {
                    ModelState.AddModelError("Input.Email", "Этот email уже зарегистрирован");
                }
                    
                // Если не удалось определить конкретную ошибку, показываем общее сообщение
                if (ModelState.ErrorCount == 0)
                {
                    ModelState.AddModelError(string.Empty, "Произошла ошибка при регистрации. Пожалуйста, проверьте введенные данные.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Произошла ошибка при регистрации. Пожалуйста, попробуйте позже.");
            }
        }

        return Page();
    }
} 