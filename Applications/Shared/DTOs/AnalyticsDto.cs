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
        public List<TopProductResultDto> TopProducts { get; set; } = new();

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
        public List<CategorySalesResultDto> CategorySales { get; set; } = new();

        [Display(Name = "Динамика продаж")]
        public List<SalesTrendResultDto> SalesTrend { get; set; } = new();
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
        [Display(Name = "ID отчета")]
        public int Id { get; set; }

        [Display(Name = "Тип отчета")]
        [Required(ErrorMessage = "Тип отчета обязателен")]
        public ReportType Type { get; set; }

        [Display(Name = "Название отчета")]
        [Required(ErrorMessage = "Название отчета обязательно")]
        [StringLength(100, ErrorMessage = "Название отчета не должно превышать 100 символов")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Описание")]
        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Период")]
        [Required(ErrorMessage = "Период обязателен")]
        [StringLength(50, ErrorMessage = "Период не должен превышать 50 символов")]
        public string Period { get; set; } = string.Empty;

        [Display(Name = "Дата создания")]
        [Required(ErrorMessage = "Дата создания обязательна")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Создатель")]
        [Required(ErrorMessage = "Создатель обязателен")]
        [StringLength(100, ErrorMessage = "Имя создателя не должно превышать 100 символов")]
        public string CreatedBy { get; set; } = string.Empty;

        [Display(Name = "Версия")]
        [Required(ErrorMessage = "Версия обязательна")]
        [StringLength(20, ErrorMessage = "Версия не должна превышать 20 символов")]
        public string Version { get; set; } = string.Empty;

        [Display(Name = "Формат")]
        [Required(ErrorMessage = "Формат обязателен")]
        public ReportFormat Format { get; set; }

        [Display(Name = "Настройки форматирования")]
        [Required(ErrorMessage = "Настройки форматирования обязательны")]
        public ReportFormattingSettings FormattingSettings { get; set; } = new();

        [Display(Name = "Данные отчета")]
        [Required(ErrorMessage = "Данные отчета обязательны")]
        public ReportDataDto Data { get; set; } = new();
    }

    /// <summary>
    /// DTO для данных отчета
    /// </summary>
    public class ReportDataDto
    {
        [Display(Name = "Метрики")]
        public Dictionary<string, decimal> Metrics { get; set; } = new();

        [Display(Name = "Таблицы")]
        public List<ReportTableDto> Tables { get; set; } = new();

        [Display(Name = "Графики")]
        public List<ReportChartDto> Charts { get; set; } = new();
    }

    /// <summary>
    /// DTO для таблицы в отчете
    /// </summary>
    public class ReportTableDto
    {
        [Display(Name = "Название")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Заголовки")]
        public List<string> Headers { get; set; } = new();

        [Display(Name = "Строки")]
        public List<List<object>> Rows { get; set; } = new();

        [Display(Name = "Итоги")]
        public Dictionary<string, object> Totals { get; set; } = new();
    }

    /// <summary>
    /// DTO для графика в отчете
    /// </summary>
    public class ReportChartDto
    {
        [Display(Name = "Название")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Тип")]
        public ChartType Type { get; set; }

        [Display(Name = "Данные")]
        public Dictionary<string, List<decimal>> Data { get; set; } = new();

        [Display(Name = "Метки")]
        public List<string> Labels { get; set; } = new();
    }

    /// <summary>
    /// Настройки форматирования отчета
    /// </summary>
    public class ReportFormattingSettings
    {
        [Display(Name = "Язык")]
        public string Language { get; set; } = "ru-RU";

        [Display(Name = "Валюта")]
        public string Currency { get; set; } = "BYN";

        [Display(Name = "Формат даты")]
        public string DateFormat { get; set; } = "dd.MM.yyyy";

        [Display(Name = "Формат чисел")]
        public string NumberFormat { get; set; } = "N2";

        [Display(Name = "Шаблон")]
        public string Template { get; set; } = "Default";

        [Display(Name = "Стили")]
        public Dictionary<string, string> Styles { get; set; } = new();
    }

    /// <summary>
    /// Тип графика
    /// </summary>
    public enum ChartType
    {
        [Display(Name = "Линейный")]
        Line,

        [Display(Name = "Столбчатый")]
        Bar,

        [Display(Name = "Круговой")]
        Pie,

        [Display(Name = "Область")]
        Area,

        [Display(Name = "Точечный")]
        Scatter
    }

    /// <summary>
    /// DTO для топа товаров
    /// </summary>
    public class TopProductResultDto
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
    public class CategorySalesResultDto
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
    public class SalesTrendResultDto
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

    /// <summary>
    /// DTO для расширенной аналитики продаж
    /// </summary>
    public class ExtendedSalesAnalyticsDto
    {
        [Display(Name = "Конверсия продаж")]
        public decimal ConversionRate { get; set; }
        
        [Display(Name = "Среднее время выполнения заказа")]
        public TimeSpan AverageOrderProcessingTime { get; set; }

        [Display(Name = "Эффективность складов")]
        public List<StockEfficiencyResultDto> StockEfficiency { get; set; } = new();

        [Display(Name = "Сезонность продаж")]
        public List<SeasonalityResultDto> Seasonality { get; set; } = new();

        [Display(Name = "Прогноз продаж")]
        public List<SalesForecastResultDto> SalesForecast { get; set; } = new();
    }

    /// <summary>
    /// DTO для результата эффективности склада
    /// </summary>
    public class StockEfficiencyResultDto
    {
        [Display(Name = "Название склада")]
        public string StockName { get; set; } = string.Empty;

        [Display(Name = "Оборот")]
        public decimal Turnover { get; set; }

        [Display(Name = "Коэффициент оборачиваемости")]
        public decimal TurnoverRatio { get; set; }
    }

    /// <summary>
    /// DTO для результата сезонности
    /// </summary>
    public class SeasonalityResultDto
    {
        [Display(Name = "Период")]
        public string Period { get; set; } = string.Empty;

        [Display(Name = "Средний объем продаж")]
        public decimal AverageSales { get; set; }

        [Display(Name = "Отклонение от среднего")]
        public decimal Deviation { get; set; }

        [Display(Name = "Сезонный коэффициент")]
        public decimal SeasonalityIndex { get; set; }
    }

    /// <summary>
    /// DTO для результата прогноза продаж
    /// </summary>
    public class SalesForecastResultDto
    {
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Display(Name = "Прогноз продаж")]
        public decimal ForecastedSales { get; set; }

        [Display(Name = "Доверительный интервал (нижний)")]
        public decimal LowerBound { get; set; }

        [Display(Name = "Доверительный интервал (верхний)")]
        public decimal UpperBound { get; set; }
    }

    /// <summary>
    /// DTO для KPI
    /// </summary>
    public class KpiDto
    {
        [Display(Name = "Конверсия продаж")]
        public decimal SalesConversion { get; set; }

        [Display(Name = "Среднее время выполнения заказа")]
        public TimeSpan AverageOrderTime { get; set; }

        [Display(Name = "Выручка")]
        public decimal Revenue { get; set; }

        [Display(Name = "Объем продаж")]
        public int SalesVolume { get; set; }
        
        [Display(Name = "Средний чек")]
        public decimal AverageCheck { get; set; }

        [Display(Name = "Оборачиваемость склада")]
        public decimal StockTurnover { get; set; }

        [Display(Name = "Средний срок выполнения заказа")]
        public TimeSpan AverageOrderProcessingTime { get; set; }
    }

    /// <summary>
    /// DTO для прогноза спроса
    /// </summary>
    public class DemandForecastDto
    {
        [Display(Name = "Товар")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "Категория")]
        public string Category { get; set; } = string.Empty;

        [Display(Name = "Прогноз спроса")]
        public int ForecastedQuantity { get; set; }

        [Display(Name = "Прогноз выручки")]
        public decimal ForecastedRevenue { get; set; }
                
        [Display(Name = "Доверительный интервал (нижний)")]
        public int LowerBound { get; set; }

        [Display(Name = "Доверительный интервал (верхний)")]
        public int UpperBound { get; set; }

        [Display(Name = "Текущий остаток")]
        public int CurrentStock { get; set; }

        [Display(Name = "Рекомендуемый заказ")]
        public int RecommendedOrder { get; set; }

        [Display(Name = "Уверенность в прогнозе")]
        public decimal Confidence { get; set; }

        [Display(Name = "Сообщение")]
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO для анализа влияния сезонности
    /// </summary>
    public class SeasonalityImpactDto
    {
        [Display(Name = "Категория")]
        public string Category { get; set; } = string.Empty;

        [Display(Name = "Сезонный коэффициент")]
        public decimal SeasonalityIndex { get; set; }

        [Display(Name = "Пиковый месяц")]
        public string PeakMonth { get; set; } = string.Empty;

        [Display(Name = "Спад")]
        public string LowMonth { get; set; } = string.Empty;

        [Display(Name = "Влияние на продажи")]
        public decimal Impact { get; set; }
    }

    /// <summary>
    /// DTO для ABC-анализа по выручке
    /// </summary>
    public class ProductAbcAnalysisDto
    {
        [Display(Name = "Название товара")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "Выручка")]
        public decimal Revenue { get; set; }

        [Display(Name = "Доля в общей выручке")]
        public decimal Share { get; set; }

        [Display(Name = "Накопительная доля")]
        public decimal CumulativeShare { get; set; }

        [Display(Name = "Категория")]
        public string Category { get; set; } = string.Empty;
    }
} 