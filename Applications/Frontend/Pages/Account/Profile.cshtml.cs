using Application.DTOs;
using Frontend.Services.Account;
using Frontend.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly IClientService _clientService;

    public ProfileModel(IAuthService authService, IClientService clientService)
    {
        _authService = authService;
        _clientService = clientService;
    }

    public UserDto CurrentUser { get; set; }
    [BindProperty]
    public ProfileFormDto ClientData { get; set; }

    [BindProperty]
    public ChangePasswordDto ChangePasswordData { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            CurrentUser = await _authService.GetCurrentUserAsync();
            if (CurrentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Получаем данные клиента, если они есть
            try
            {
                var client = await _clientService.GetCurrentClientAsync();
                ClientData = new ProfileFormDto
                {
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Phone = client.Phone
                };
            }
            catch
            {
                // Если клиент не найден, создаем пустой объект
                ClientData = new ProfileFormDto
                {
                    FirstName = "",
                    LastName = "",
                    Phone = ""
                };
            }

            return Page();
        }
        catch
        {
            return RedirectToPage("/Account/Login");
        }
    }

    public async Task<IActionResult> OnPostUpdateProfileAsync()
    {
        // Очищаем ModelState и проверяем только поля профиля
        ModelState.Clear();
        TryValidateModel(ClientData);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            CurrentUser = await _authService.GetCurrentUserAsync();
            if (CurrentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Проверяем, существует ли клиент
            if (await _clientService.ExistsForCurrentUserAsync())
            {
                // Обновляем существующего клиента
                await _clientService.UpdateSelfAsync(new UpdateClientDto
                {
                    FirstName = ClientData.FirstName,
                    LastName = ClientData.LastName,
                    Phone = ClientData.Phone
                });
            }
            else
            {
                // Создаем нового клиента
                await _clientService.CreateForCurrentUserAsync(new CreateClientDto
                {
                    FirstName = ClientData.FirstName,
                    LastName = ClientData.LastName,
                    Phone = ClientData.Phone
                });
            }

            TempData["SuccessMessage"] = "Данные успешно сохранены";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        // Очищаем ModelState и проверяем только поля пароля
        ModelState.Clear();
        TryValidateModel(ChangePasswordData);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var response = await _clientService.ChangePasswordAsync(ChangePasswordData);
            if (response.Success)
            {
                TempData["success"] = response.Message;
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            TempData["error"] = "Произошла ошибка при смене пароля";
            return Page();
        }
    }
    
    public class ProfileFormDto
    {
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Имя клиента обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Фамилия клиента обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; }

        [Display(Name = "Телефон")]
        [Required(ErrorMessage = "Телефон клиента обязателен")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string Phone { get; set; }
    }
} 