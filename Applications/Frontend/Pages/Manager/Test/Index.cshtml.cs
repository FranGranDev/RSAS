using System.ComponentModel.DataAnnotations;
using Application.DTOs.Test;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Manager.Test;

[Authorize(Policy = "RequireManagerRole")]
public class IndexModel : PageModel
{
    private readonly IApiService _apiService;

    public IndexModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public GenerateSalesDto GenerateSalesDto { get; set; } = new()
    {
        StartDate = DateTime.UtcNow.AddDays(-30),
        EndDate = DateTime.UtcNow,
        SalesCount = 10,
        AverageOrderDurationMinutes = 60,
        MinProductsPerOrder = 1,
        MaxProductsPerOrder = 5,
        MinProductQuantity = 1,
        MaxProductQuantity = 10,
        MinDeliveryDays = 1,
        MaxDeliveryDays = 4
    };

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _apiService.PostAsync<object, GenerateSalesDto>("api/testdata/generate-sales", GenerateSalesDto);
            TempData["success"] = "Тестовые продажи успешно сгенерированы";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
            return Page();
        }
    }
} 