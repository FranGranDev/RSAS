using Application.Data;
using Application.Models;
using Microsoft.EntityFrameworkCore;

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
                .Include(s => s.Order)
                .Include(s => s.Stock)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Sale>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(s => s.Order)
                .Include(s => s.Stock)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByOrderIdAsync(int orderId)
        {
            return await _dbSet
                .Include(s => s.Order)
                .Include(s => s.Stock)
                .Where(s => s.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByStockIdAsync(int stockId)
        {
            return await _dbSet
                .Include(s => s.Order)
                .Include(s => s.Stock)
                .Where(s => s.StockId == stockId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByStatusAsync(SaleStatus status)
        {
            return await _dbSet
                .Include(s => s.Order)
                .Include(s => s.Stock)
                .Where(s => s.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(s => s.Order)
                .Include(s => s.Stock)
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsByOrderIdAsync(int orderId)
        {
            return await _dbSet.AnyAsync(s => s.OrderId == orderId);
        }
    }
}