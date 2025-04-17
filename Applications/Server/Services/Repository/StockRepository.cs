using Application.Data;
using Application.Models;
using Application.Services.Repository;
using Microsoft.EntityFrameworkCore;

namespace Server.Services.Repository
{
    public class StockRepository : Repository<Stock, int>, IStockRepository
    {
        public StockRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Stock> GetByNameAsync(string name)
        {
            return await _context.Stocks
                .FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<Stock> GetByAddressAsync(string address)
        {
            return await _context.Stocks
                .FirstOrDefaultAsync(s => s.Address == address);
        }

        public async Task<IEnumerable<Stock>> GetByCityAsync(string city)
        {
            return await _context.Stocks
                .Where(s => s.City == city)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Stocks
                .AnyAsync(s => s.Name == name);
        }

        public async Task<bool> ExistsByAddressAsync(string address)
        {
            return await _context.Stocks
                .AnyAsync(s => s.Address == address);
        }

        public async Task<Stock> GetWithStockProductsAsync(int id)
        {
            return await _context.Stocks
                .Include(s => s.StockProducts)
                .ThenInclude(sp => sp.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Stock>> GetAllWithStockProductsAsync()
        {
            return await _context.Stocks
                .Include(s => s.StockProducts)
                .ThenInclude(sp => sp.Product)
                .ToListAsync();
        }

        public async Task<StockProducts> GetStockProductAsync(int stockId, int productId)
        {
            return await _context.StockProducts
                .FirstOrDefaultAsync(sp => sp.StockId == stockId && sp.ProductId == productId);
        }

        public async Task UpdateStockProductAsync(StockProducts stockProduct)
        {
            _context.StockProducts.Update(stockProduct);
            await _context.SaveChangesAsync();
        }
    }
}