using System.ComponentModel;

namespace Application.ViewModel.Sales
{
    public class SalesAnalyticsViewModel
    {
        [DisplayName("Период продаж")]
        public string Period { get; set; }

        [DisplayName("Выручка")]
        public decimal Revenue { get; set; }

        [DisplayName("Количество продаж")]
        public int SalesCount { get; set; }

        [DisplayName("Проданные товары")]
        public int ProductsCount { get; set; }

        [DisplayName("Средний чек")]
        public decimal AvgRevenue { get; set; }
    }
}
