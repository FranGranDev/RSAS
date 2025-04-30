using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using Application.Utils;
using AutoMapper;
using Server.Models;
using Server.Services.Repository;

namespace Server.Services.Sales
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public SaleService(
            ISaleRepository saleRepository, 
            IOrderRepository orderRepository,
            IMapper mapper)
        {
            _saleRepository = saleRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<SaleDto> GetByIdAsync(int id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null)
                throw new SaleNotFoundException(id);

            return _mapper.Map<SaleDto>(sale);
        }

        public async Task<IEnumerable<SaleDto>> GetAllAsync()
        {
            var sales = await _saleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<SaleDto> CreateFromOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetWithDetailsAsync(orderId);
            if (order == null)
                throw new OrderNotFoundException(orderId);

            if (await _saleRepository.ExistsByOrderIdAsync(orderId))
                throw new BusinessException($"Продажа для заказа {orderId} уже существует");

            // Рассчитываем общую сумму заказа
            var totalAmount = order.Products.Sum(op => op.ProductPrice * op.Quantity);

            var sale = new Sale
            {
                OrderId = orderId,
                SaleDate = SystemTime.Now,
                TotalAmount = totalAmount,
                ClientName = order.ClientName,
                ClientPhone = order.ContactPhone,
                PaymentMethod = order.PaymentType.ToString(),
                Products = order.Products.Select(op => new SaleProduct
                {
                    ProductId = op.ProductId,
                    ProductName = op.ProductName,
                    ProductCategory = op.Product.Category, // Берем категорию из Product
                    Quantity = op.Quantity,
                    ProductPrice = op.ProductPrice,
                    DiscountAmount = 0 // Пока не реализовано
                }).ToList()
            };

            await _saleRepository.AddAsync(sale);
            return _mapper.Map<SaleDto>(sale);
        }

        public async Task<bool> ExistsByOrderIdAsync(int orderId)
        {
            return await _saleRepository.ExistsByOrderIdAsync(orderId);
        }

        public async Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new InvalidDateRangeException("Дата начала не может быть позже даты окончания");

            var sales = await _saleRepository.GetByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetByClientAsync(string clientPhone)
        {
            var sales = await _saleRepository.GetByClientAsync(clientPhone);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetByProductAsync(int productId)
        {
            var sales = await _saleRepository.GetByProductAsync(productId);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetByCategoryAsync(string category)
        {
            var sales = await _saleRepository.GetByCategoryAsync(category);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            ValidateDateRange(startDate, endDate);
            return await _saleRepository.GetTotalRevenueAsync(startDate, endDate);
        }

        public async Task<int> GetTotalSalesCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            ValidateDateRange(startDate, endDate);
            return await _saleRepository.GetTotalSalesCountAsync(startDate, endDate);
        }

        public async Task<decimal> GetAverageSaleAmountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            ValidateDateRange(startDate, endDate);
            return await _saleRepository.GetAverageSaleAmountAsync(startDate, endDate);
        }

        public async Task<IEnumerable<TopProductResultDto>> GetTopProductsAsync(
            int count = 10,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            ValidateDateRange(startDate, endDate);
            if (count <= 0)
                throw new InvalidAnalyticsParametersException("Количество товаров должно быть больше 0");

            return await _saleRepository.GetTopProductsAsync(count, startDate, endDate);
        }

        public async Task<IEnumerable<CategorySalesResultDto>> GetCategorySalesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            ValidateDateRange(startDate, endDate);
            return await _saleRepository.GetCategorySalesAsync(startDate, endDate);
        }

        public async Task<IEnumerable<SalesTrendResultDto>> GetSalesTrendAsync(
            DateTime startDate,
            DateTime endDate,
            string interval = "1d")
        {
            if (startDate > endDate)
                throw new InvalidDateRangeException("Дата начала не может быть позже даты окончания");

            TimeSpan timeSpan;
            if (interval.EndsWith("d"))
            {
                if (!int.TryParse(interval[..^1], out var days))
                {
                    throw new InvalidAnalyticsParametersException("Неверный формат интервала. Используйте формат '1d', '2d' и т.д.");
                }
                timeSpan = TimeSpan.FromDays(days);
            }
            else if (interval.EndsWith("h"))
            {
                if (!int.TryParse(interval[..^1], out var hours))
                {
                    throw new InvalidAnalyticsParametersException("Неверный формат интервала. Используйте формат '1h', '2h' и т.д.");
                }
                timeSpan = TimeSpan.FromHours(hours);
            }
            else if (interval.EndsWith("m"))
            {
                if (!int.TryParse(interval[..^1], out var minutes))
                {
                    throw new InvalidAnalyticsParametersException("Неверный формат интервала. Используйте формат '1m', '2m' и т.д.");
                }
                timeSpan = TimeSpan.FromMinutes(minutes);
            }
            else
            {
                throw new InvalidAnalyticsParametersException("Неверный формат интервала. Используйте формат '1d', '1h', '1m' и т.д.");
            }

            if (timeSpan <= TimeSpan.Zero)
                throw new InvalidAnalyticsParametersException("Интервал должен быть больше 0");

            return await _saleRepository.GetSalesTrendAsync(startDate, endDate, timeSpan);
        }

        public async Task<DashboardAnalyticsDto> GetDashboardAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            ValidateDateRange(startDate, endDate);
            
            var totalRevenue = await GetTotalRevenueAsync(startDate, endDate);
            var totalSalesCount = await GetTotalSalesCountAsync(startDate, endDate);
            var averageOrderAmount = await GetAverageSaleAmountAsync(startDate, endDate);
            var topProducts = await GetTopProductsAsync(5, startDate, endDate);

            return new DashboardAnalyticsDto
            {
                TotalRevenue = totalRevenue,
                TotalSalesCount = totalSalesCount,
                AverageOrderAmount = averageOrderAmount,
                TopProducts = topProducts.ToList()
            };
        }

        public async Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            ValidateDateRange(startDate, endDate);

            var revenue = await GetTotalRevenueAsync(startDate, endDate);
            var salesCount = await GetTotalSalesCountAsync(startDate, endDate);
            var averageSaleAmount = await GetAverageSaleAmountAsync(startDate, endDate);
            var categorySales = await GetCategorySalesAsync(startDate, endDate);
            var salesTrend = await GetSalesTrendAsync(
                startDate ?? DateTime.MinValue,
                endDate ?? DateTime.MaxValue,
                "1d");

            return new SalesAnalyticsDto
            {
                Period = GetPeriodString(startDate, endDate),
                Revenue = revenue,
                SalesCount = salesCount,
                AverageSaleAmount = averageSaleAmount,
                CategorySales = categorySales.ToList(),
                SalesTrend = salesTrend.ToList()
            };
        }

        public async Task<OrdersAnalyticsDto> GetOrdersAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            ValidateDateRange(startDate, endDate);

            // Получаем заказы за период
            var orders = await _orderRepository.GetByDateRangeAsync(
                startDate ?? DateTime.MinValue,
                endDate ?? DateTime.MaxValue);

            var ordersCount = orders.Count();
            var averageOrderAmount = orders.Any() 
                ? orders.Average(o => o.Products.Sum(op => op.ProductPrice * op.Quantity))
                : 0;
            var averageProcessingTime = orders.Any() 
                ? TimeSpan.FromTicks((long)orders.Average(o => (o.ChangeDate - o.OrderDate).Ticks))
                : TimeSpan.Zero;

            // Статистика по статусам
            var statusStats = new OrderStatusStatsDto
            {
                NewCount = orders.Count(o => o.State == Order.States.New),
                ProcessingCount = orders.Count(o => o.State == Order.States.InProcess),
                CompletedCount = orders.Count(o => o.State == Order.States.Completed),
                CancelledCount = orders.Count(o => o.State == Order.States.Cancelled)
            };

            // Причины отмены
            var cancellationReasons = orders
                .Where(o => o.State == Order.States.Cancelled)
                .GroupBy(o => o.CancellationReason ?? "Не указана")
                .Select(g => new CancellationReasonDto
                {
                    Reason = g.Key,
                    Count = g.Count(),
                    Share = (decimal)g.Count() / orders.Count(o => o.State == Order.States.Cancelled)
                })
                .ToList();

            return new OrdersAnalyticsDto
            {
                Period = GetPeriodString(startDate, endDate),
                OrdersCount = ordersCount,
                AverageOrderAmount = averageOrderAmount,
                AverageProcessingTime = averageProcessingTime,
                StatusStats = statusStats,
                CancellationReasons = cancellationReasons
            };
        }

        public async Task<ExtendedSalesAnalyticsDto> GetExtendedAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            ValidateDateRange(startDate, endDate);

            var conversionRate = await _saleRepository.GetSalesConversionRateAsync(startDate, endDate);
            var averageOrderProcessingTime = await _saleRepository.GetAverageOrderProcessingTimeAsync(startDate, endDate);
            var stockEfficiency = await _saleRepository.GetStockEfficiencyAsync(startDate, endDate);
            var seasonality = await _saleRepository.GetSeasonalityAsync();
            var salesForecast = await _saleRepository.GetSalesForecastAsync();

            return new ExtendedSalesAnalyticsDto
            {
                ConversionRate = conversionRate,
                AverageOrderProcessingTime = averageOrderProcessingTime,
                StockEfficiency = stockEfficiency.ToList(),
                Seasonality = seasonality.ToList(),
                SalesForecast = salesForecast.ToList()
            };
        }

        public async Task<KpiDto> GetKpiAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            ValidateDateRange(startDate, endDate);
            return await _saleRepository.GetKpiAsync(startDate, endDate);
        }

        public async Task<ReportDto> GenerateReportAsync(
            ReportType type,
            ReportFormat format,
            DateTime? startDate = null,
            DateTime? endDate = null,
            ReportFormattingSettings? formattingSettings = null,
            string? userId = null,
            string? userName = null)
        {
            ValidateDateRange(startDate, endDate);

            var report = new ReportDto
            {
                Type = type,
                Format = format,
                Period = GetPeriodString(startDate, endDate),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userName ?? "System",
                Version = "1.0",
                FormattingSettings = formattingSettings ?? new ReportFormattingSettings(),
                Data = new ReportDataDto()
            };

            switch (type)
            {
                case ReportType.Sales:
                    var salesAnalytics = await GetSalesAnalyticsAsync(startDate, endDate);
                    report.Data.Metrics = new Dictionary<string, decimal>
                    {
                        { "Выручка", salesAnalytics.Revenue },
                        { "Количество продаж", salesAnalytics.SalesCount },
                        { "Средний чек", salesAnalytics.AverageSaleAmount }
                    };
                    report.Data.Tables = new List<ReportTableDto>
                    {
                        new ReportTableDto
                        {
                            Title = "Продажи по категориям",
                            Headers = new List<string> { "Категория", "Количество", "Выручка", "Доля" },
                            Rows = salesAnalytics.CategorySales.Select(cs => new List<object>
                            {
                                cs.Category,
                                cs.SalesCount,
                                cs.Revenue,
                                cs.Share
                            }).ToList()
                        }
                    };
                    report.Data.Charts = new List<ReportChartDto>
                    {
                        new ReportChartDto
                        {
                            Title = "Динамика продаж",
                            Type = ChartType.Line,
                            Data = new Dictionary<string, List<decimal>>
                            {
                                { "Выручка", salesAnalytics.SalesTrend.Select(st => st.Revenue).ToList() }
                            },
                            Labels = salesAnalytics.SalesTrend.Select(st => st.Date.ToString("dd.MM.yyyy")).ToList()
                        }
                    };
                    break;

                case ReportType.Orders:
                    var ordersAnalytics = await GetOrdersAnalyticsAsync(startDate, endDate);
                    report.Data.Metrics = new Dictionary<string, decimal>
                    {
                        { "Количество заказов", ordersAnalytics.OrdersCount },
                        { "Средний чек", ordersAnalytics.AverageOrderAmount },
                        { "Среднее время обработки", (decimal)ordersAnalytics.AverageProcessingTime.TotalHours }
                    };
                    report.Data.Tables = new List<ReportTableDto>
                    {
                        new ReportTableDto
                        {
                            Title = "Статистика по статусам",
                            Headers = new List<string> { "Статус", "Количество", "Доля" },
                            Rows = new List<List<object>>
                            {
                                new List<object> { "Новые", ordersAnalytics.StatusStats.NewCount, 
                                    ordersAnalytics.OrdersCount > 0 ? (decimal)ordersAnalytics.StatusStats.NewCount / ordersAnalytics.OrdersCount : 0 },
                                new List<object> { "В обработке", ordersAnalytics.StatusStats.ProcessingCount,
                                    ordersAnalytics.OrdersCount > 0 ? (decimal)ordersAnalytics.StatusStats.ProcessingCount / ordersAnalytics.OrdersCount : 0 },
                                new List<object> { "Завершенные", ordersAnalytics.StatusStats.CompletedCount,
                                    ordersAnalytics.OrdersCount > 0 ? (decimal)ordersAnalytics.StatusStats.CompletedCount / ordersAnalytics.OrdersCount : 0 },
                                new List<object> { "Отмененные", ordersAnalytics.StatusStats.CancelledCount,
                                    ordersAnalytics.OrdersCount > 0 ? (decimal)ordersAnalytics.StatusStats.CancelledCount / ordersAnalytics.OrdersCount : 0 }
                            }
                        }
                    };
                    break;

                case ReportType.KPI:
                    var kpi = await GetKpiAsync(startDate, endDate);
                    report.Data.Metrics = new Dictionary<string, decimal>
                    {
                        { "Конверсия продаж", kpi.SalesConversion },
                        { "Выручка", kpi.Revenue },
                        { "Объем продаж", kpi.SalesVolume },
                        { "Средний чек", kpi.AverageCheck },
                        { "Оборачиваемость склада", kpi.StockTurnover }
                    };
                    break;

                default:
                    throw new InvalidAnalyticsParametersException("Неподдерживаемый тип отчета");
            }

            return report;
        }

        public async Task<ReportDto> GenerateExtendedReportAsync(
            ReportType type,
            ReportFormat format,
            DateTime? startDate = null,
            DateTime? endDate = null,
            ReportFormattingSettings? formattingSettings = null,
            string? userId = null,
            string? userName = null)
        {
            var report = await GenerateReportAsync(type, format, startDate, endDate, formattingSettings, userId, userName);

            var extendedAnalytics = await GetExtendedAnalyticsAsync(startDate, endDate);
            
            report.Data.Metrics.Add("Конверсия продаж", extendedAnalytics.ConversionRate);

            report.Data.Tables.Add(new ReportTableDto
            {
                Title = "Эффективность складов",
                Headers = new List<string> { "Склад", "Оборот", "Коэффициент оборачиваемости" },
                Rows = extendedAnalytics.StockEfficiency.Select(se => new List<object>
                {
                    se.StockName,
                    se.Turnover,
                    se.TurnoverRatio
                }).ToList()
            });

            report.Data.Charts.Add(new ReportChartDto
            {
                Title = "Сезонность продаж",
                Type = ChartType.Bar,
                Data = new Dictionary<string, List<decimal>>
                {
                    { "Средний объем продаж", extendedAnalytics.Seasonality.Select(s => s.AverageSales).ToList() }
                },
                Labels = extendedAnalytics.Seasonality.Select(s => s.Period).ToList()
            });

            return report;
        }

        public async Task<IEnumerable<CategoryForecastDto>> GetCategoryForecastAsync(
            int days = 30,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            if (days <= 0)
                throw new InvalidAnalyticsParametersException("Количество дней должно быть больше 0");

            ValidateDateRange(startDate, endDate);
            return await _saleRepository.GetCategoryForecastAsync(days, startDate, endDate);
        }

        public async Task<IEnumerable<DemandForecastDto>> GetDemandForecastAsync(
            int days = 30,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            if (days <= 0)
                throw new InvalidAnalyticsParametersException("Количество дней должно быть больше 0");

            ValidateDateRange(startDate, endDate);
            return await _saleRepository.GetDemandForecastAsync(days, startDate, endDate);
        }

        public async Task<IEnumerable<SeasonalityImpactDto>> GetSeasonalityImpactAsync(
            int years = 3,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            if (years <= 0)
                throw new InvalidAnalyticsParametersException("Количество лет должно быть больше 0");

            ValidateDateRange(startDate, endDate);
            return await _saleRepository.GetSeasonalityImpactAsync(years, startDate, endDate);
        }

        private void ValidateDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new InvalidDateRangeException("Дата начала не может быть позже даты окончания");
        }

        private string GetPeriodString(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue && !endDate.HasValue)
                return "За все время";
            if (!startDate.HasValue)
                return $"До {endDate.Value:dd.MM.yyyy}";
            if (!endDate.HasValue)
                return $"С {startDate.Value:dd.MM.yyyy}";
            return $"{startDate.Value:dd.MM.yyyy} - {endDate.Value:dd.MM.yyyy}";
        }
    }
}