using Application.Data;
using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Services.Repository
{
    public class OrderRepository : Repository<Order, int>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Order> GetWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAllWithDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .ThenInclude(p => p.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .ThenInclude(p => p.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByStockIdAsync(int stockId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .ThenInclude(p => p.Product)
                .Where(o => o.StockId == stockId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByStateAsync(Order.States state)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .ThenInclude(p => p.Product)
                .Where(o => o.State == state)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .ThenInclude(p => p.Product)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsByUserIdAsync(string userId)
        {
            return await _context.Orders
                .AnyAsync(o => o.UserId == userId);
        }
    }
}