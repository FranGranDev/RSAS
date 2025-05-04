using System.Globalization;
using Application.Data;
using Application.DTOs;
using Application.Utils;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Services.Repository
{
    public class SaleRepository : Repository<Sale, int>, ISaleRepository
    {
        public SaleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Sale> GetWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Sale>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(s => s.Products)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByOrderIdAsync(int orderId)
        {
            return await _dbSet
                .Include(s => s.Products)
                .Where(s => s.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(s => s.Products)
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByClientAsync(string clientPhone)
        {
            return await _dbSet
                .Include(s => s.Products)
                .Where(s => s.ClientPhone == clientPhone)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByProductAsync(int productId)
        {
            return await _dbSet
                .Include(s => s.Products)
                .Where(s => s.Products.Any(sp => sp.ProductId == productId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByCategoryAsync(string category)
        {
            return await _dbSet
                .Include(s => s.Products)
                .Where(s => s.Products.Any(sp => sp.ProductCategory == category))
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);

            return await query.SumAsync(s => s.TotalAmount);
        }

        public async Task<int> GetTotalSalesCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<decimal> GetAverageSaleAmountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);

            return await query.AverageAsync(s => s.TotalAmount);
        }

        public async Task<IEnumerable<TopProductResultDto>> GetTopProductsAsync(
            int count = 10,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _dbSet
                .Include(s => s.Products)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);

            var result = await query
                .SelectMany(s => s.Products)
                .GroupBy(sp => new { sp.ProductId, sp.ProductName })
                .Select(g => new TopProductResultDto
                {
                    ProductName = g.Key.ProductName,
                    SalesCount = g.Sum(sp => sp.Quantity),
                    Revenue = g.Sum(sp => sp.ProductPrice * sp.Quantity)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(count)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<CategorySalesResultDto>> GetCategorySalesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _dbSet
                .Include(s => s.Products)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);

            var totalRevenue = await query
                .SelectMany(s => s.Products)
                .SumAsync(sp => sp.ProductPrice * sp.Quantity);

            var result = await query
                .SelectMany(s => s.Products)
                .GroupBy(sp => sp.ProductCategory)
                .Select(g => new CategorySalesResultDto
                {
                    Category = g.Key,
                    SalesCount = g.Sum(sp => sp.Quantity),
                    Revenue = g.Sum(sp => sp.ProductPrice * sp.Quantity),
                    Share = totalRevenue > 0 ? g.Sum(sp => sp.ProductPrice * sp.Quantity) / totalRevenue : 0
                })
                .OrderByDescending(x => x.Revenue)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<SalesTrendResultDto>> GetSalesTrendAsync(
            DateTime startDate,
            DateTime endDate,
            TimeSpan interval)
        {
            // Получаем все продажи за период
            var sales = await _dbSet
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .ToListAsync();

            // Группируем продажи по интервалам
            var result = sales
                .GroupBy(s =>
                {
                    if (interval.TotalHours >= 1)
                    {
                        // Группировка по часам
                        return new DateTime(
                            s.SaleDate.Year,
                            s.SaleDate.Month,
                            s.SaleDate.Day,
                            s.SaleDate.Hour / (int)interval.TotalHours * (int)interval.TotalHours,
                            0, 0);
                    }
                    else if (interval.TotalMinutes >= 1)
                    {
                        // Группировка по минутам
                        return new DateTime(
                            s.SaleDate.Year,
                            s.SaleDate.Month,
                            s.SaleDate.Day,
                            s.SaleDate.Hour,
                            s.SaleDate.Minute / (int)interval.TotalMinutes * (int)interval.TotalMinutes,
                            0);
                    }
                    else
                    {
                        // Группировка по дням
                        return s.SaleDate.Date;
                    }
                })
                .Select(g => new SalesTrendResultDto
                {
                    Date = g.Key,
                    SalesCount = g.Count(),
                    Revenue = g.Sum(s => s.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<decimal> GetSalesConversionRateAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);

            var totalOrders = await _context.Orders.CountAsync();
            var completedSales = await query.CountAsync();

            return totalOrders > 0 ? (decimal)completedSales / totalOrders : 0;
        }

        public async Task<TimeSpan> GetAverageOrderProcessingTimeAsync(DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _dbSet
                .Include(s => s.Order)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);

            var sales = await query.ToListAsync();

            if (!sales.Any())
                return TimeSpan.Zero;

            var averageTicks = sales.Average(s => (s.SaleDate - s.Order.OrderDate).Ticks);
            return TimeSpan.FromTicks((long)averageTicks);
        }

        public async Task<IEnumerable<StockEfficiencyResultDto>> GetStockEfficiencyAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _dbSet
                .Include(s => s.Products)
                .ThenInclude(sp => sp.Product)
                .ThenInclude(p => p.StockProducts)
                .ThenInclude(sp => sp.Stock)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);

            // Получаем все продажи с продуктами и их связями со складами
            var salesWithProducts = await query
                .SelectMany(s => s.Products)
                .Select(sp => new
                {
                    Product = sp.Product,
                    StockProducts = sp.Product.StockProducts
                })
                .ToListAsync();

            // Группируем по складам и считаем метрики
            var result = salesWithProducts
                .SelectMany(x => x.StockProducts)
                .GroupBy(sp => new { sp.Stock.Id, sp.Stock.Name })
                .Select(g => new StockEfficiencyResultDto
                {
                    StockName = g.Key.Name,
                    Turnover = g.Sum(sp => sp.Product.Price * sp.Quantity),
                    TurnoverRatio = g.Count() / (decimal)g.Select(sp => sp.Product.Id).Distinct().Count()
                })
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SeasonalityResultDto>> GetSeasonalityAsync(int years = 3)
        {
            var endDate = SystemTime.Now;
            ;
            var startDate = endDate.AddYears(-years);

            var monthlyData = await _dbSet
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                .Select(g => new
                {
                    Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Revenue = g.Sum(s => s.TotalAmount)
                })
                .ToListAsync();

            var averageRevenue = monthlyData.Average(x => x.Revenue);

            return monthlyData.Select(x => new SeasonalityResultDto
            {
                Period = x.Period,
                AverageSales = x.Revenue,
                Deviation = x.Revenue - averageRevenue,
                SeasonalityIndex = averageRevenue > 0 ? x.Revenue / averageRevenue : 0
            });
        }

        public async Task<IEnumerable<SalesForecastResultDto>> GetSalesForecastAsync(int days = 30)
        {
            var endDate = SystemTime.Now;
            ;
            var startDate = endDate.AddMonths(-3); // Используем данные за последние 3 месяца

            var historicalData = await _dbSet
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(s => s.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // Простой прогноз на основе среднего и стандартного отклонения
            var average = historicalData.Average(x => x.Revenue);
            var stdDev = Math.Sqrt(historicalData.Average(x => Math.Pow((double)(x.Revenue - average), 2)));

            var forecast = new List<SalesForecastResultDto>();
            for (int i = 1; i <= days; i++)
            {
                var date = endDate.AddDays(i);
                forecast.Add(new SalesForecastResultDto
                {
                    Date = date,
                    ForecastedSales = average,
                    LowerBound = average - (decimal)stdDev,
                    UpperBound = average + (decimal)stdDev
                });
            }

            return forecast;
        }

        public async Task<KpiDto> GetKpiAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return new KpiDto
            {
                SalesConversion = await GetSalesConversionRateAsync(startDate, endDate),
                AverageOrderTime = await GetAverageOrderProcessingTimeAsync(startDate, endDate),
                Revenue = await GetTotalRevenueAsync(startDate, endDate),
                SalesVolume = await GetTotalSalesCountAsync(startDate, endDate),
                AverageCheck = await GetAverageSaleAmountAsync(startDate, endDate),
                StockTurnover = (await GetStockEfficiencyAsync(startDate, endDate))
                    .Average(x => x.TurnoverRatio),
                AverageOrderProcessingTime = await GetAverageOrderProcessingTimeAsync(startDate, endDate)
            };
        }

        public async Task<bool> ExistsByOrderIdAsync(int orderId)
        {
            return await _dbSet.AnyAsync(s => s.OrderId == orderId);
        }

        private class HistoricalSalesData
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string Category { get; set; }
            public List<DailySalesData> DailySales { get; set; }
            public decimal TotalQuantity { get; set; }
            public decimal TotalRevenue { get; set; }
        }

        private class DailySalesData
        {
            public DateTime Date { get; set; }
            public int Quantity { get; set; }
            public decimal Revenue { get; set; }
        }

        public async Task<IEnumerable<DemandForecastDto>> GetDemandForecastAsync(
            int days = 30,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var endDateValue = endDate ?? SystemTime.Now;
            var startDateValue = startDate ?? endDateValue.AddMonths(-6);

            // Получаем исторические данные о продажах
            var historicalSales = await _dbSet
                .Include(s => s.Products)
                .Where(s => s.SaleDate >= startDateValue && s.SaleDate <= endDateValue)
                .SelectMany(s => s.Products)
                .GroupBy(sp => new { sp.ProductId, sp.ProductName, sp.ProductCategory })
                .Select(g => new HistoricalSalesData
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    Category = g.Key.ProductCategory,
                    TotalQuantity = g.Sum(sp => sp.Quantity),
                    TotalRevenue = g.Sum(sp => sp.ProductPrice * sp.Quantity),
                    DailySales = g.GroupBy(sp => sp.Sale.SaleDate.Date)
                        .Select(gr => new DailySalesData
                        {
                            Date = gr.Key,
                            Quantity = gr.Sum(x => x.Quantity),
                            Revenue = gr.Sum(x => x.ProductPrice * x.Quantity)
                        })
                        .OrderBy(x => x.Date)
                        .ToList()
                })
                .ToListAsync();

            // Получаем текущие остатки на складах
            var currentStock = await _context.StockProducts
                .Include(sp => sp.Product)
                .GroupBy(sp => new { sp.ProductId, sp.Product.Name })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    CurrentStock = g.Sum(sp => sp.Quantity)
                })
                .ToDictionaryAsync(x => x.ProductId, x => x.CurrentStock);

            var forecast = new List<DemandForecastDto>();
            foreach (var product in historicalSales)
            {
                if (!product.DailySales.Any())
                    continue;

                // 1. Взвешенное скользящее среднее (Weighted Moving Average)
                var wma = CalculateWeightedMovingAverage(product.DailySales.Select(x => x.Quantity).ToList());

                // 2. Экспоненциальное сглаживание (Exponential Smoothing)
                var alpha = 0.3m; // Коэффициент сглаживания
                var es = CalculateExponentialSmoothing(product.DailySales.Select(x => x.Quantity).ToList(), alpha);

                // 3. Сезонная декомпозиция (Seasonal Decomposition)
                var seasonalFactors = CalculateSeasonalFactors(product.DailySales);

                // 4. Комбинированный прогноз
                var combinedForecast = (wma + es) / 2;
                var forecastedQuantity = (int)Math.Ceiling(combinedForecast * days);

                // 5. Расчет прогнозируемой выручки с учетом сезонности
                var averagePrice = product.TotalRevenue / product.TotalQuantity;
                var forecastedRevenue = forecastedQuantity * averagePrice;

                // 6. Расчет доверительного интервала с использованием t-статистики
                var (lowerBound, upperBound) = CalculateConfidenceInterval(
                    product.DailySales.Select(x => x.Quantity).ToList(),
                    forecastedQuantity,
                    days);

                // 7. Расчет страхового запаса с учетом сезонности и волатильности
                var safetyStock = CalculateSafetyStock(
                    product.DailySales.Select(x => x.Quantity).ToList(),
                    seasonalFactors,
                    days);

                // 8. Расчет рекомендуемого заказа
                var currentStockQuantity = currentStock.GetValueOrDefault(product.ProductId, 0);
                var recommendedOrder = Math.Max(0, forecastedQuantity + safetyStock - currentStockQuantity);

                // 9. Расчет уверенности в прогнозе
                var confidence = CalculateForecastConfidence(
                    product.DailySales.Count,
                    seasonalFactors,
                    product.DailySales.Select(x => x.Quantity).ToList());

                // 10. Формирование сообщения о состоянии запасов
                var message = GenerateStockStatusMessage(
                    currentStockQuantity,
                    forecastedQuantity,
                    safetyStock,
                    confidence);

                forecast.Add(new DemandForecastDto
                {
                    ProductName = product.ProductName,
                    Category = product.Category,
                    ForecastedQuantity = forecastedQuantity,
                    ForecastedRevenue = forecastedRevenue,
                    LowerBound = lowerBound,
                    UpperBound = upperBound,
                    CurrentStock = currentStockQuantity,
                    RecommendedOrder = recommendedOrder,
                    Confidence = confidence,
                    Message = message
                });
            }

            return forecast;
        }

        private decimal CalculateWeightedMovingAverage(List<int> data)
        {
            if (data.Count < 3) return (decimal)(data.Count != 0 ? data.Average() : 0);

            var weights = new[] { 0.5m, 0.3m, 0.2m }; // Веса для последних 3 дней
            var sum = 0m;
            var weightSum = 0m;

            for (int i = 0; i < Math.Min(3, data.Count); i++)
            {
                sum += data[data.Count - 1 - i] * weights[i];
                weightSum += weights[i];
            }

            return sum / weightSum;
        }

        private decimal CalculateExponentialSmoothing(List<int> data, decimal alpha)
        {
            if (!data.Any()) return 0;

            var smoothed = (decimal)data[0];
            for (int i = 1; i < data.Count; i++)
            {
                smoothed = alpha * data[i] + (1 - alpha) * smoothed;
            }

            return smoothed;
        }

        private Dictionary<DayOfWeek, decimal> CalculateSeasonalFactors(List<DailySalesData> dailySales)
        {
            var factors = new Dictionary<DayOfWeek, decimal>();
            var weeklyTotals = new Dictionary<DayOfWeek, List<int>>();

            foreach (var sale in dailySales)
            {
                var dayOfWeek = sale.Date.DayOfWeek;
                if (!weeklyTotals.ContainsKey(dayOfWeek))
                    weeklyTotals[dayOfWeek] = new List<int>();

                weeklyTotals[dayOfWeek].Add(sale.Quantity);
            }

            var overallAverage = weeklyTotals.Values.SelectMany(x => x).Average();
            if (overallAverage == 0) return factors;

            foreach (var day in weeklyTotals)
            {
                var dayAverage = day.Value.Average();
                factors[day.Key] = (decimal)(dayAverage / overallAverage);
            }

            return factors;
        }

        private (int lower, int upper) CalculateConfidenceInterval(List<int> data, decimal forecast, int days)
        {
            if (!data.Any()) return (0, 0);

            var mean = data.Average();
            var stdDev = Math.Sqrt(data.Average(x => Math.Pow(x - mean, 2)));
            var tValue = 1.96; // Для 95% доверительного интервала

            var margin = (decimal)(tValue * stdDev * Math.Sqrt(days));
            var lower = Math.Max(0, (int)(forecast - margin));
            var upper = (int)(forecast + margin);

            return (lower, upper);
        }

        private int CalculateSafetyStock(List<int> data, Dictionary<DayOfWeek, decimal> seasonalFactors, int days)
        {
            if (!data.Any() || !seasonalFactors.Any()) return 0;

            var stdDev = Math.Sqrt(data.Average(x => Math.Pow(x - data.Average(), 2)));
            var maxSeasonalFactor = seasonalFactors.Values.Max();
            var leadTime = 7; // Время выполнения заказа в днях

            return (int)Math.Ceiling(stdDev * (double)maxSeasonalFactor * Math.Sqrt(leadTime));
        }

        private decimal CalculateForecastConfidence(int dataPoints, Dictionary<DayOfWeek, decimal> seasonalFactors, List<int> data)
        {
            if (!data.Any()) return 0;

            // Базовый уровень уверенности на основе количества данных
            var baseConfidence = Math.Min(1.0m, (decimal)dataPoints / 30);

            // Учет сезонности
            var seasonalConfidence = seasonalFactors.Values.Any() 
                ? seasonalFactors.Values.Average() 
                : 1.0m;

            // Учет волатильности
            var mean = data.Average();
            var stdDev = Math.Sqrt(data.Average(x => Math.Pow(x - mean, 2)));
            var volatilityConfidence = mean > 0 
                ? Math.Max(0, 1 - (decimal)(stdDev / mean)) 
                : 0;

            return (baseConfidence + seasonalConfidence + volatilityConfidence) / 3;
        }

        private string GenerateStockStatusMessage(int currentStock, int forecastedQuantity, int safetyStock, decimal confidence)
        {
            if (currentStock <= 0) return "Нет в наличии";
            if (currentStock < forecastedQuantity) return "Недостаточно запасов";
            if (currentStock < forecastedQuantity + safetyStock) return "Рекомендуется пополнение";
            if (confidence < 0.5m) return "Запасы в норме (низкая уверенность в прогнозе)";
            return "Запасы в норме";
        }

        public async Task<IEnumerable<SeasonalityImpactDto>> GetSeasonalityImpactAsync(
            int years = 3,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var endDateValue = endDate ?? SystemTime.Now;
            ;
            var startDateValue = startDate ?? endDateValue.AddYears(-years);

            // Получаем месячные данные по категориям
            var monthlyData = await _dbSet
                .Include(s => s.Products)
                .Where(s => s.SaleDate >= startDateValue && s.SaleDate <= endDateValue)
                .SelectMany(s => s.Products)
                .GroupBy(sp => new { sp.ProductCategory, Month = sp.Sale.SaleDate.Month })
                .Select(g => new
                {
                    Category = g.Key.ProductCategory,
                    Month = g.Key.Month,
                    Revenue = g.Sum(sp => sp.ProductPrice * sp.Quantity)
                })
                .ToListAsync();

            // Анализируем сезонность для каждой категории
            var result = monthlyData
                .GroupBy(x => x.Category)
                .Select(g =>
                {
                    var monthlyAverages = g
                        .GroupBy(x => x.Month)
                        .Select(mg => new
                        {
                            Month = mg.Key,
                            AverageRevenue = mg.Average(x => x.Revenue)
                        })
                        .ToList();

                    var overallAverage = monthlyAverages.Average(x => x.AverageRevenue);
                    var peakMonth = monthlyAverages.OrderByDescending(x => x.AverageRevenue).First();
                    var lowMonth = monthlyAverages.OrderBy(x => x.AverageRevenue).First();

                    return new SeasonalityImpactDto
                    {
                        Category = g.Key,
                        SeasonalityIndex = peakMonth.AverageRevenue / overallAverage,
                        PeakMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(peakMonth.Month),
                        LowMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(lowMonth.Month),
                        Impact = (peakMonth.AverageRevenue - lowMonth.AverageRevenue) / overallAverage
                    };
                })
                .ToList();

            return result;
        }
    }
}