using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Sales;
using System.Security.Claims;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        /// <summary>
        /// Получить продажу по ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<SaleDto>> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID должен быть больше 0");
            }

            try
            {
                var sale = await _saleService.GetByIdAsync(id);
                return Ok(sale);
            }
            catch (SaleNotFoundException)
            {
                return NotFound($"Продажа с ID {id} не найдена");
            }
        }

        /// <summary>
        /// Получить все продажи
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetAll()
        {
            var sales = await _saleService.GetAllAsync();
            return Ok(sales);
        }

        /// <summary>
        /// Создать продажу из заказа
        /// </summary>
        [HttpPost("from-order/{orderId}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<SaleDto>> CreateFromOrder(int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest("ID заказа должен быть больше 0");
            }

            try
            {
                var sale = await _saleService.CreateFromOrderAsync(orderId);
                return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
            }
            catch (OrderNotFoundException)
            {
                return NotFound($"Заказ с ID {orderId} не найден");
            }
            catch (InvalidOrderStateException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить аналитику продаж
        /// </summary>
        [HttpGet("analytics")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<SalesAnalyticsDto>> GetSalesAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var analytics = await _saleService.GetSalesAnalyticsAsync(startDate, endDate);
                return Ok(analytics);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить аналитику заказов
        /// </summary>
        [HttpGet("orders-analytics")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<OrdersAnalyticsDto>> GetOrdersAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var analytics = await _saleService.GetOrdersAnalyticsAsync(startDate, endDate);
                return Ok(analytics);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить аналитику для дашборда
        /// </summary>
        [HttpGet("dashboard")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<DashboardAnalyticsDto>> GetDashboardAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var analytics = await _saleService.GetDashboardAnalyticsAsync(startDate, endDate);
                return Ok(analytics);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить прогноз спроса
        /// </summary>
        [HttpGet("forecast/demand/days/{days}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<DemandForecastDto>>> GetDemandForecast(
            [FromRoute] int days = 30,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (days <= 0)
            {
                return BadRequest("Количество дней должно быть больше 0");
            }

            if (days > 365)
            {
                return BadRequest("Прогноз не может быть больше чем на год");
            }

            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var forecast = await _saleService.GetDemandForecastAsync(days, startDate, endDate);
                return Ok(forecast);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить анализ влияния сезонности
        /// </summary>
        [HttpGet("seasonality/years/{years}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<SeasonalityImpactDto>>> GetSeasonalityImpact(
            [FromRoute] int years = 3,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (years <= 0)
            {
                return BadRequest("Количество лет должно быть больше 0");
            }

            if (years > 10)
            {
                return BadRequest("Анализ не может быть больше чем за 10 лет");
            }

            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var impact = await _saleService.GetSeasonalityImpactAsync(years, startDate, endDate);
                return Ok(impact);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Сгенерировать отчет
        /// </summary>
        [HttpPost("report")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ReportDto>> GenerateReport(
            [FromQuery] ReportType type,
            [FromQuery] ReportFormat format,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromBody] ReportFormattingSettings? formattingSettings = null)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;

                var report = await _saleService.GenerateReportAsync(
                    type,
                    format,
                    startDate,
                    endDate,
                    formattingSettings,
                    userId,
                    userName);

                return Ok(report);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidAnalyticsParametersException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Сгенерировать расширенный отчет
        /// </summary>
        [HttpPost("report/extended")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ReportDto>> GenerateExtendedReport(
            [FromQuery] ReportType type,
            [FromQuery] ReportFormat format,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromBody] ReportFormattingSettings? formattingSettings = null)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;

                var report = await _saleService.GenerateExtendedReportAsync(
                    type,
                    format,
                    startDate,
                    endDate,
                    formattingSettings,
                    userId,
                    userName);

                return Ok(report);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidAnalyticsParametersException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить расширенную аналитику
        /// </summary>
        [HttpGet("extended")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<ExtendedSalesAnalyticsDto>> GetExtendedAnalytics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var analytics = await _saleService.GetExtendedAnalyticsAsync(startDate, endDate);
                return Ok(analytics);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить KPI
        /// </summary>
        [HttpGet("kpi")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<KpiDto>> GetKpi(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var kpi = await _saleService.GetKpiAsync(startDate, endDate);
                return Ok(kpi);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить топ продуктов
        /// </summary>
        [HttpGet("top-products")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<TopProductResultDto>>> GetTopProducts(
            [FromQuery] int count = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (count <= 0)
            {
                return BadRequest("Количество продуктов должно быть больше 0");
            }

            if (count > 100)
            {
                return BadRequest("Количество продуктов не может быть больше 100");
            }

            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var topProducts = await _saleService.GetTopProductsAsync(count, startDate, endDate);
                return Ok(topProducts);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить продажи по категориям
        /// </summary>
        [HttpGet("category-sales")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<CategorySalesResultDto>>> GetCategorySales(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var categorySales = await _saleService.GetCategorySalesAsync(startDate, endDate);
                return Ok(categorySales);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить тренд продаж
        /// </summary>
        [HttpGet("trend")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<SalesTrendResultDto>>> GetSalesTrend(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string interval = "1d")
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var trend = await _saleService.GetSalesTrendAsync(
                    startDate ?? DateTime.UtcNow.AddDays(-30), 
                    endDate ?? DateTime.UtcNow, 
                    interval);
                return Ok(trend);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить ABC-анализ по выручке
        /// </summary>
        [HttpGet("abc-analysis")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<ActionResult<IEnumerable<ProductAbcAnalysisDto>>> GetProductAbcAnalysis(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest("Начальная дата не может быть позже конечной");
            }

            if (startDate.HasValue && startDate > DateTime.UtcNow)
            {
                return BadRequest("Начальная дата не может быть в будущем");
            }

            try
            {
                var analysis = await _saleService.GetProductAbcAnalysisAsync(startDate, endDate);
                return Ok(analysis);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}