using Application.DTOs;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Manager.Stocks;

[Authorize(Roles = "Manager")]
public class CreateModel : PageModel
{
    private readonly IApiService _apiService;

    public CreateModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public CreateStockDto Stock { get; set; }

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
            await _apiService.PostAsync<StockDto, CreateStockDto>("api/stocks", Stock);
            TempData["success"] = "Склад успешно создан";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Ошибка при создании склада: " + ex.Message;
            return Page();
        }
    }
} 