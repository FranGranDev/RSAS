using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Frontend.Services;

namespace Frontend.Pages.Orders
{
    public class EditModel : PageModel
    {
        private readonly ApiService _apiService;

        public EditModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public OrderViewModel Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Order = await _apiService.GetOrderAsync(id);
                if (Order == null)
                {
                    return NotFound();
                }
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
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
                var result = await _apiService.UpdateOrderAsync(Order.Id, Order);
                if (result)
                {
                    return RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ошибка при обновлении заказа");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Произошла ошибка при обновлении заказа");
                return Page();
            }
        }
    }
} 