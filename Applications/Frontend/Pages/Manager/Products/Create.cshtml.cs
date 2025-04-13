using Application.DTOs;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;

namespace Frontend.Pages.Manager.Products;

[Authorize(Roles = "Manager")]
public class CreateModel : PageModel
{
    private readonly IApiService _apiService;

    public CreateModel(IApiService apiService)
    {
        _apiService = apiService;
        Product = new ProductDto
        {
            Barcode = GenerateEan13()
        };
    }

    [BindProperty]
    public ProductDto Product { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _apiService.PostAsync<ProductDto, ProductDto>("api/products", Product);
            TempData["success"] = "Товар успешно создан";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Ошибка при создании товара: " + ex.Message;
            return Page();
        }
    }

    private string GenerateEan13()
    {
        // Генерируем 12 случайных цифр
        var random = new Random();
        var digits = new int[12];
        for (int i = 0; i < 12; i++)
        {
            digits[i] = random.Next(0, 10);
        }

        // Вычисляем контрольную сумму
        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            sum += digits[i] * (i % 2 == 0 ? 1 : 3);
        }
        int checkDigit = (10 - (sum % 10)) % 10;

        // Формируем итоговый штрих-код
        return string.Join("", digits) + checkDigit;
    }
} 