using Application.Data;
using Application.Model.Stocks;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Repository
{
    public class EFStockProductsStore : IStockProductsStore
    {
        private readonly AppDbContext _context;

        public EFStockProductsStore(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<StockProduct> All => _context.StockProducts;

        public StockProduct Get(int id)
        {
            return _context.StockProducts
                .Include(sp => sp.Product)
                .Include(sp => sp.Stock)
                .FirstOrDefault(sp => sp.Id == id);
        }

        public void Save(StockProduct entity)
        {
            if (entity.Id == 0)
            {
                _context.StockProducts.Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }

        public void Delete(StockProduct entity)
        {
            _context.StockProducts.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<StockProduct>> GetAllAsync()
        {
            return await _context.StockProducts
                .Include(sp => sp.Product)
                .Include(sp => sp.Stock)
                .ToListAsync();
        }

        public async Task<StockProduct> GetByIdAsync(int id)
        {
            return await _context.StockProducts
                .Include(sp => sp.Product)
                .Include(sp => sp.Stock)
                .FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task<StockProduct> SaveAsync(StockProduct entity)
        {
            if (entity.Id == 0)
            {
                _context.StockProducts.Add(entity);
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
            var stockProduct = await _context.StockProducts
                .Include(sp => sp.Product)
                .Include(sp => sp.Stock)
                .FirstOrDefaultAsync(sp => sp.Id == id);
                
            if (stockProduct != null)
            {
                _context.StockProducts.Remove(stockProduct);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<StockProduct>> GetByStockIdAsync(int stockId)
        {
            return await _context.StockProducts
                .Include(sp => sp.Product)
                .Include(sp => sp.Stock)
                .Where(sp => sp.StockId == stockId)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockProduct>> GetByProductIdAsync(int productId)
        {
            return await _context.StockProducts
                .Include(sp => sp.Product)
                .Include(sp => sp.Stock)
                .Where(sp => sp.ProductId == productId)
                .ToListAsync();
        }

        public async Task<StockProduct> GetByStockAndProductIdAsync(int stockId, int productId)
        {
            return await _context.StockProducts
                .Include(sp => sp.Product)
                .Include(sp => sp.Stock)
                .FirstOrDefaultAsync(sp => sp.StockId == stockId && sp.ProductId == productId);
        }
    }
}