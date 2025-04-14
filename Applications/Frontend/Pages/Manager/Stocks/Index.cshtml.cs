using Application.DTOs;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Manager.Stocks;

[Authorize(Roles = "Manager")]
public class IndexModel : PageModel
{
    private readonly IApiService _apiService;

    public IndexModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public IEnumerable<StockDto> Stocks { get; set; } = new List<StockDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            Stocks = await _apiService.GetAsync<IEnumerable<StockDto>>("api/stocks");
            return Page();
        }
        catch (Exception ex)
        {
            TempData["error"] = "Ошибка при загрузке складов: " + ex.Message;
            return Page();
        }
    }
} 