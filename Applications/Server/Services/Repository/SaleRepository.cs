using Application.Data;
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

        public async Task<IEnumerable<(string ProductName, int SalesCount, decimal Revenue)>> GetTopProductsAsync(
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
                .Select(g => new
                {
                    ProductName = g.Key.ProductName,
                    SalesCount = g.Sum(sp => sp.Quantity),
                    Revenue = g.Sum(sp => sp.ProductPrice * sp.Quantity)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(count)
                .ToListAsync();

            return result.Select(x => (x.ProductName, x.SalesCount, x.Revenue));
        }

        public async Task<IEnumerable<(string Category, int SalesCount, decimal Revenue, decimal Share)>> GetCategorySalesAsync(
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
                .Select(g => new
                {
                    Category = g.Key,
                    SalesCount = g.Sum(sp => sp.Quantity),
                    Revenue = g.Sum(sp => sp.ProductPrice * sp.Quantity),
                    Share = totalRevenue > 0 ? g.Sum(sp => sp.ProductPrice * sp.Quantity) / totalRevenue : 0
                })
                .OrderByDescending(x => x.Revenue)
                .ToListAsync();

            return result.Select(x => (x.Category, x.SalesCount, x.Revenue, x.Share));
        }

        public async Task<IEnumerable<(DateTime Date, int SalesCount, decimal Revenue)>> GetSalesTrendAsync(
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
                .Select(g => new
                {
                    Date = g.Key,
                    SalesCount = g.Count(),
                    Revenue = g.Sum(s => s.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return result.Select(x => (x.Date, x.SalesCount, x.Revenue));
        }

        public async Task<bool> ExistsByOrderIdAsync(int orderId)
        {
            return await _dbSet.AnyAsync(s => s.OrderId == orderId);
        }
    }
}