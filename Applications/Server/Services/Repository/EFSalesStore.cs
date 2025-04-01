using Application.Data;
using Application.Model.Orders;
using Application.Model.Sales;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Repository
{
    public class EFSalesStore : ISalesStore
    {
        private readonly AppDbContext _context;

        public EFSalesStore(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Sale> All => _context.Sales;

        public Sale Get(int id)
        {
            return _context.Sales
                .Include(s => s.Products)
                .FirstOrDefault(s => s.Id == id);
        }

        public void Save(Sale entity)
        {
            if (entity.Id == 0)
            {
                _context.Sales.Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }

        public void Delete(Sale entity)
        {
            _context.Sales.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            return await _context.Sales
                .Include(s => s.Products)
                .ToListAsync();
        }

        public async Task<Sale> GetByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sale> SaveAsync(Sale entity)
        {
            if (entity.Id == 0)
            {
                _context.Sales.Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id);
                
            if (sale != null)
            {
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Sale>> GetByStockIdAsync(int stockId)
        {
            return await _context.Sales
                .Include(s => s.Products)
                .Where(s => s.StockId == stockId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Include(s => s.Products)
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .ToListAsync();
        }

        public async Task CreateSaleAsync(Order order)
        {
            if (order.State != Order.States.Completed)
            {
                throw new InvalidOperationException("Можно создавать продажу только из завершенного заказа");
            }

            if (!order.StockId.HasValue)
            {
                throw new InvalidOperationException("Не указан склад");
            }

            var sale = new Sale
            {
                OrderId = order.Id,
                StockId = order.StockId.Value,
                SaleDate = DateTime.UtcNow,
                SaleType = order.Type
            };

            await SaveAsync(sale);
        }
    }
}