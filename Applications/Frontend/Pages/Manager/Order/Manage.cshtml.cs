using Application.DTOs;
using Frontend.Pages.Manager.Stocks;
using Frontend.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Manager.Order;

[Authorize(Roles = "Manager")]
public class ManageModel : PageModel
{
    private readonly IApiService _apiService;
    private readonly ILogger<ViewModel> _logger;
    
    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }
    
    public OrderWithStockInfoDto Order { get; set; }
    public IEnumerable<StockDto> Stocks { get; set; }
    
    public ManageModel(IApiService apiService, ILogger<ViewModel> logger)
    {
        _logger = logger;
        _apiService = apiService;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            // Сначала получаем список складов
            Stocks = await _apiService.GetAsync<IEnumerable<StockDto>>("/api/stocks");
            
            if (!Stocks.Any())
            {
                _logger.LogError("Не найдено ни одного склада");
                return RedirectToPage("/Error");
            }
            
            // Используем ID первого склада для загрузки данных о наличии товаров
            var firstStockId = Stocks.First().Id;
            Order = await _apiService.GetAsync<OrderWithStockInfoDto>($"/api/orders/{Id}/stock-info?stockId={firstStockId}");
            
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке страницы управления заказом {Id}", Id);
            return RedirectToPage("/Error");
        }
    }
    
    public async Task<IActionResult> OnGetOrderProductsAsync(int stockId)
    {
        try
        {
            var order = await _apiService.GetAsync<OrderWithStockInfoDto>($"/api/orders/{Id}/stock-info?stockId={stockId}");
            return Partial("Shared/Orders/_ManageOrderProductPartial", order.Products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке страницы управления заказом {Id}", Id);
            return RedirectToPage("/Error");
        }
    }
    
    public async Task<IActionResult> OnPostUpdateDeliveryDateAsync([FromBody] UpdateDeliveryDateRequest request)
    {
        try
        {
            // Получаем информацию о доставке
            var delivery = await _apiService.GetAsync<DeliveryDto>($"/api/orders/{request.orderId}/delivery");
            
            if (delivery == null)
            {
                return new JsonResult(new { success = false, error = "Доставка не найдена" });
            }
            
            // Создаем DTO для обновления с текущими данными
            var updateDeliveryDto = new UpdateDeliveryDto
            {
                City = delivery.City,
                Street = delivery.Street,
                House = delivery.House,
                Flat = delivery.Flat,
                PostalCode = delivery.PostalCode,
                DeliveryDate = request.deliveryDate // Обновляем только дату
            };

            // Отправляем обновленные данные
            await _apiService.PutAsync<object, UpdateDeliveryDto>(
                $"/api/orders/{request.orderId}/delivery",
                updateDeliveryDto);
            
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении даты доставки заказа {OrderId}", request.orderId);
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }
    
    public async Task<IActionResult> OnPostExecuteOrderAsync([FromBody] ExecuteOrderRequest request)
    {
        try
        {
            await _apiService.PostAsync<object, int>($"/api/orders/{request.OrderId}/execute", request.StockId);
            TempData["success"] = "Заказ успешно подготовлен к выдаче";
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при выполнении заказа {OrderId} на складе {StockId}", request.OrderId, request.StockId);
            TempData["error"] = ex.Message;
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }
    
    public async Task<IActionResult> OnPostCancelOrderAsync([FromBody] CancelOrderRequest request)
    {
        try
        {
            await _apiService.PostAsync<object, object>($"/api/orders/{request.orderId}/cancel", null);
            TempData["success"] = "Заказ успешно отменен";
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }
    
    public async Task<IActionResult> OnPostCompleteOrderAsync([FromBody] CompleteOrderRequest request)
    {
        try
        {
            await _apiService.PostAsync<object, object>($"/api/orders/{request.orderId}/complete", null);
            TempData["success"] = "Заказ успешно завершен";
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }

    public async Task<IActionResult> OnPostHoldOrderAsync([FromBody] HoldOrderRequest request)
    {
        try
        {
            await _apiService.PostAsync<object, object>($"/api/orders/{request.orderId}/hold", null);
            TempData["success"] = "Заказ успешно отложен";
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }

    public async Task<IActionResult> OnPostResumeOrderAsync([FromBody] ResumeOrderRequest request)
    {
        try
        {
            await _apiService.PostAsync<object, object>($"/api/orders/{request.orderId}/resume", null);
            TempData["success"] = "Заказ успешно возобновлен";
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }

    public async Task<IActionResult> OnDeleteDeleteOrderAsync([FromBody] DeleteOrderRequest request)
    {
        try
        {
            await _apiService.DeleteAsync($"/api/orders/{request.orderId}");
            TempData["success"] = "Заказ успешно удален";
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении заказа {OrderId}", request.orderId);
            TempData["error"] = ex.Message;
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }
}

// Класс для десериализации запроса
public class UpdateDeliveryDateRequest
{
    public int orderId { get; set; }
    public DateTime deliveryDate { get; set; }
}

public class ExecuteOrderRequest
{
    public int OrderId { get; set; }
    public int StockId { get; set; }
}

public class DeleteOrderRequest
{
    public int orderId { get; set; }
}

public class CancelOrderRequest
{
    public int orderId { get; set; }
}

public class CompleteOrderRequest
{
    public int orderId { get; set; }
}

public class HoldOrderRequest
{
    public int orderId { get; set; }
}

public class ResumeOrderRequest
{
    public int orderId { get; set; }
} 