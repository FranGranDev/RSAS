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

        public async Task<decimal> GetTotalCostAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet
                .Include(s => s.Products)
                .AsQueryable();
            
            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);
                
            return await query
                .SelectMany(s => s.Products)
                .SumAsync(sp => sp.ProductPrice * sp.Quantity);
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
            var query = _dbSet.AsQueryable()
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate);

            var result = await query
                .GroupBy(s => new DateTime(
                    s.SaleDate.Year,
                    s.SaleDate.Month,
                    s.SaleDate.Day,
                    s.SaleDate.Hour / interval.Hours * interval.Hours,
                    0, 0))
                .Select(g => new SalesTrendResultDto
                {
                    Date = g.Key,
                    SalesCount = g.Count(),
                    Revenue = g.Sum(s => s.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

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

        public async Task<decimal> GetGrossProfitAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var revenue = await GetTotalRevenueAsync(startDate, endDate);
            var cost = await GetTotalCostAsync(startDate, endDate);
            return revenue - cost;
        }

        public async Task<decimal> GetProfitMarginAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var revenue = await GetTotalRevenueAsync(startDate, endDate);
            var profit = await GetGrossProfitAsync(startDate, endDate);
            return revenue > 0 ? profit / revenue : 0;
        }

        public async Task<TimeSpan> GetAverageOrderProcessingTimeAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.AsQueryable();
            
            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);

            var averageTicks = await query
                .Select(s => (s.SaleDate - s.Order.OrderDate).Ticks)
                .AverageAsync();

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
            var endDate = SystemTime.Now;;
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
            var endDate = SystemTime.Now;;
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
                GrossProfit = await GetGrossProfitAsync(startDate, endDate),
                AverageCheck = await GetAverageSaleAmountAsync(startDate, endDate),
                ProfitMargin = await GetProfitMarginAsync(startDate, endDate),
                StockTurnover = (await GetStockEfficiencyAsync(startDate, endDate))
                    .Average(x => x.TurnoverRatio),
                AverageOrderProcessingTime = await GetAverageOrderProcessingTimeAsync(startDate, endDate)
            };
        }

        public async Task<bool> ExistsByOrderIdAsync(int orderId)
        {
            return await _dbSet.AnyAsync(s => s.OrderId == orderId);
        }

        public async Task<IEnumerable<CategoryForecastDto>> GetCategoryForecastAsync(
            int days = 30,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var endDateValue = endDate ?? SystemTime.Now;;
            var startDateValue = startDate ?? endDateValue.AddMonths(-3);

            // Получаем исторические данные по категориям
            var historicalData = await _dbSet
                .Include(s => s.Products)
                .Where(s => s.SaleDate >= startDateValue && s.SaleDate <= endDateValue)
                .SelectMany(s => s.Products)
                .GroupBy(sp => new { sp.ProductCategory, Date = sp.Sale.SaleDate.Date })
                .Select(g => new
                {
                    Category = g.Key.ProductCategory,
                    Date = g.Key.Date,
                    Revenue = g.Sum(sp => sp.ProductPrice * sp.Quantity)
                })
                .ToListAsync();

            // Группируем по категориям для прогноза
            var result = historicalData
                .GroupBy(x => x.Category)
                .Select(g => new CategoryForecastDto
                {
                    Category = g.Key,
                    ForecastedSales = g.Average(x => x.Revenue) * days,
                    LowerBound = g.Average(x => x.Revenue) * days * 0.8m, // -20%
                    UpperBound = g.Average(x => x.Revenue) * days * 1.2m, // +20%
                    Confidence = 0.8m // Примерная точность
                })
                .ToList();

            return result;
        }

        public async Task<IEnumerable<DemandForecastDto>> GetDemandForecastAsync(
            int days = 30,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var endDateValue = endDate ?? SystemTime.Now;;
            var startDateValue = startDate ?? endDateValue.AddMonths(-3);

            // Получаем исторические данные по продуктам
            var historicalData = await _dbSet
                .Include(s => s.Products)
                    .ThenInclude(sp => sp.Product)
                        .ThenInclude(p => p.StockProducts)
                .Where(s => s.SaleDate >= startDateValue && s.SaleDate <= endDateValue)
                .SelectMany(s => s.Products)
                .GroupBy(sp => new { sp.ProductId, sp.ProductName, sp.ProductCategory })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    Category = g.Key.ProductCategory,
                    AverageDailySales = g.Average(sp => sp.Quantity),
                    CurrentStock = g.First().Product.StockProducts.Sum(sp => sp.Quantity)
                })
                .ToListAsync();

            // Создаем прогноз для каждого продукта
            var result = historicalData
                .Select(x => new DemandForecastDto
                {
                    ProductName = x.ProductName,
                    Category = x.Category,
                    ForecastedQuantity = (int)(x.AverageDailySales * days),
                    LowerBound = (int)(x.AverageDailySales * days * 0.8), // -20%
                    UpperBound = (int)(x.AverageDailySales * days * 1.2), // +20%
                    CurrentStock = x.CurrentStock,
                    RecommendedOrder = Math.Max(0, (int)(x.AverageDailySales * days) - x.CurrentStock)
                })
                .ToList();

            return result;
        }

        public async Task<IEnumerable<SeasonalityImpactDto>> GetSeasonalityImpactAsync(
            int years = 3,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var endDateValue = endDate ?? SystemTime.Now;;
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