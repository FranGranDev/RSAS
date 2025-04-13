using Application.DTOs;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Manager.Products;

[Authorize(Roles = "Manager")]
public class IndexModel : PageModel
{
    private readonly IApiService _apiService;

    public IndexModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    public IEnumerable<string> Categories { get; set; } = new List<string>();

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            // Получаем все товары
            Products = await _apiService.GetAsync<IEnumerable<ProductDto>>("api/products");
            
            // Получаем уникальные категории
            Categories = Products.Select(p => p.Category).Distinct();
            
            return Page();
        }
        catch (Exception ex)
        {
            TempData["error"] = "Ошибка при загрузке товаров: " + ex.Message;
            return Page();
        }
    }
} 