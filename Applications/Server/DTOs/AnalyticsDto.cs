using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    /// <summary>
    /// DTO для общей аналитики (Dashboard)
    /// </summary>
    public class DashboardAnalyticsDto
    {
        [Display(Name = "Общая выручка")]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Количество продаж")]
        public int TotalSalesCount { get; set; }

        [Display(Name = "Средний чек")]
        public decimal AverageOrderAmount { get; set; }

        [Display(Name = "Количество заказов")]
        public int TotalOrdersCount { get; set; }

        [Display(Name = "Топ товаров")]
        public List<TopProductDto> TopProducts { get; set; } = new();

        [Display(Name = "Статистика по статусам заказов")]
        public OrderStatusStatsDto OrderStatusStats { get; set; } = new();
    }

    /// <summary>
    /// DTO для анализа продаж
    /// </summary>
    public class SalesAnalyticsDto
    {
        [Display(Name = "Период")]
        public string Period { get; set; } = string.Empty;

        [Display(Name = "Выручка")]
        public decimal Revenue { get; set; }

        [Display(Name = "Количество продаж")]
        public int SalesCount { get; set; }

        [Display(Name = "Средний чек")]
        public decimal AverageSaleAmount { get; set; }

        [Display(Name = "Продажи по категориям")]
        public List<CategorySalesDto> CategorySales { get; set; } = new();

        [Display(Name = "Динамика продаж")]
        public List<SalesTrendDto> SalesTrend { get; set; } = new();
    }

    /// <summary>
    /// DTO для анализа заказов
    /// </summary>
    public class OrdersAnalyticsDto
    {
        [Display(Name = "Период")]
        public string Period { get; set; } = string.Empty;

        [Display(Name = "Количество заказов")]
        public int OrdersCount { get; set; }

        [Display(Name = "Средний чек")]
        public decimal AverageOrderAmount { get; set; }

        [Display(Name = "Время обработки заказов")]
        public TimeSpan AverageProcessingTime { get; set; }

        [Display(Name = "Статистика по статусам")]
        public OrderStatusStatsDto StatusStats { get; set; } = new();

        [Display(Name = "Причины отмен")]
        public List<CancellationReasonDto> CancellationReasons { get; set; } = new();
    }

    /// <summary>
    /// DTO для отчета
    /// </summary>
    public class ReportDto
    {
        [Display(Name = "Тип отчета")]
        public ReportType Type { get; set; }

        [Display(Name = "Период")]
        public string Period { get; set; } = string.Empty;

        [Display(Name = "Данные отчета")]
        public object Data { get; set; } = new();

        [Display(Name = "Формат")]
        public ReportFormat Format { get; set; }
    }

    /// <summary>
    /// DTO для топа товаров
    /// </summary>
    public class TopProductDto
    {
        [Display(Name = "Название товара")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "Количество продаж")]
        public int SalesCount { get; set; }

        [Display(Name = "Выручка")]
        public decimal Revenue { get; set; }
    }

    /// <summary>
    /// DTO для продаж по категориям
    /// </summary>
    public class CategorySalesDto
    {
        [Display(Name = "Категория")]
        public string Category { get; set; } = string.Empty;

        [Display(Name = "Количество продаж")]
        public int SalesCount { get; set; }

        [Display(Name = "Выручка")]
        public decimal Revenue { get; set; }

        [Display(Name = "Доля в общем объеме")]
        public decimal Share { get; set; }
    }

    /// <summary>
    /// DTO для тренда продаж
    /// </summary>
    public class SalesTrendDto
    {
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Display(Name = "Выручка")]
        public decimal Revenue { get; set; }

        [Display(Name = "Количество продаж")]
        public int SalesCount { get; set; }
    }

    /// <summary>
    /// DTO для статистики по статусам заказов
    /// </summary>
    public class OrderStatusStatsDto
    {
        [Display(Name = "Новых")]
        public int NewCount { get; set; }

        [Display(Name = "В обработке")]
        public int ProcessingCount { get; set; }

        [Display(Name = "Завершенных")]
        public int CompletedCount { get; set; }

        [Display(Name = "Отмененных")]
        public int CancelledCount { get; set; }
    }

    /// <summary>
    /// DTO для причин отмены заказов
    /// </summary>
    public class CancellationReasonDto
    {
        [Display(Name = "Причина")]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Количество")]
        public int Count { get; set; }

        [Display(Name = "Доля")]
        public decimal Share { get; set; }
    }

    /// <summary>
    /// Тип отчета
    /// </summary>
    public enum ReportType
    {
        [Display(Name = "Отчет по продажам")]
        Sales,

        [Display(Name = "Отчет по заказам")]
        Orders,

        [Display(Name = "Отчет по ключевым показателям")]
        KPI
    }

    /// <summary>
    /// Формат отчета
    /// </summary>
    public enum ReportFormat
    {
        [Display(Name = "Excel")]
        Excel,

        [Display(Name = "PDF")]
        PDF
    }
} 