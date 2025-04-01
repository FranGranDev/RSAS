using Application.Data;
using Application.Model.Stocks;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Repository
{
    public class EFStockStore : IStockStore
    {
        private readonly AppDbContext _context;

        public EFStockStore(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Stock> All => _context.Stocks;

        public Stock Get(int id)
        {
            return _context.Stocks
                .Include(s => s.StockProducts)
                .FirstOrDefault(s => s.Id == id);
        }

        public void Save(Stock entity)
        {
            if (entity.Id == 0)
            {
                _context.Stocks.Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }

        public void Delete(Stock entity)
        {
            _context.Stocks.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Stock>> GetAllAsync()
        {
            return await _context.Stocks
                .Include(s => s.StockProducts)
                .ToListAsync();
        }

        public async Task<Stock> GetByIdAsync(int id)
        {
            return await _context.Stocks
                .Include(s => s.StockProducts)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Stock> SaveAsync(Stock entity)
        {
            if (entity.Id == 0)
            {
                _context.Stocks.Add(entity);
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
            var stock = await _context.Stocks
                .Include(s => s.StockProducts)
                .FirstOrDefaultAsync(s => s.Id == id);
                
            if (stock != null)
            {
                _context.Stocks.Remove(stock);
                await _context.SaveChangesAsync();
            }
        }
    }
}