using Application.DTOs;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Manager.FastOrder;

[Authorize(Policy = "RequireManagerRole")]
public class IndexModel : PageModel
{
    private readonly IApiService _apiService;
    public IEnumerable<StockDto> Stocks { get; set; }

    public IndexModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        Stocks = await _apiService.GetAsync<IEnumerable<StockDto>>("api/Stocks");
    }
} 