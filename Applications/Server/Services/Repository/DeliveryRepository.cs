using Application.Data;
using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Services.Repository
{
    public class DeliveryRepository : Repository<Delivery, int>, IDeliveryRepository
    {
        public DeliveryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Delivery> GetByOrderIdAsync(int orderId)
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.OrderId == orderId);
        }

        public async Task<IEnumerable<Delivery>> GetByStatusAsync(string status)
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                .Where(d => d.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Delivery>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                .Where(d => d.DeliveryDate >= startDate && d.DeliveryDate <= endDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsByOrderIdAsync(int orderId)
        {
            return await _context.Deliveries
                .AnyAsync(d => d.OrderId == orderId);
        }
    }
}