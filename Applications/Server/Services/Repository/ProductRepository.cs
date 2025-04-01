using Application.Data;
using Application.Models;
using Application.Services.Repository;
using Microsoft.EntityFrameworkCore;

namespace Server.Services.Repository
{
    public class ProductRepository : Repository<Product, int>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Product> GetByNameAsync(string name)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<Product> GetByBarcodeAsync(string barcode)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Where(p => p.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Products
                .AnyAsync(p => p.Name == name);
        }

        public async Task<bool> ExistsByBarcodeAsync(string barcode)
        {
            return await _context.Products
                .AnyAsync(p => p.Barcode == barcode);
        }

        public async Task<Product> GetWithStockProductsAsync(int id)
        {
            return await _context.Products
                .Include(p => p.StockProducts)
                .ThenInclude(sp => sp.Stock)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllWithStockProductsAsync()
        {
            return await _context.Products
                .Include(p => p.StockProducts)
                .ThenInclude(sp => sp.Stock)
                .ToListAsync();
        }
    }
}