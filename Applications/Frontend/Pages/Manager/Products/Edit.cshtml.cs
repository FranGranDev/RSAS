using Application.DTOs;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Manager.Products;

[Authorize(Roles = "Manager")]
public class EditModel : PageModel
{
    private readonly IApiService _apiService;

    public EditModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public ProductDto Product { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Product = await _apiService.GetAsync<ProductDto>($"api/products/{id}");
            if (Product == null)
            {
                TempData["error"] = "Товар не найден";
                return RedirectToPage("./Index");
            }
            
            return Page();
        }
        catch (Exception ex)
        {
            TempData["error"] = "Ошибка при загрузке товара: " + ex.Message;
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _apiService.PutAsync<ProductDto, ProductDto>($"api/products/{Product.Id}", Product);

            TempData["success"] = "Товар успешно обновлен";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Ошибка при обновлении товара: " + ex.Message;
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _apiService.DeleteAsync($"api/products/{id}");
            TempData["success"] = "Товар успешно удален";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Ошибка при удалении товара: " + ex.Message;
            return RedirectToPage("./Edit", new { id });
        }
    }
} 