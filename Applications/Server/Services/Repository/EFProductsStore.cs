using Application.Data;
using Application.Model.Stocks;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Repository
{
    public class EFProductsStore : IProductsStore
    {
        private readonly AppDbContext _context;

        public EFProductsStore(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Product> All => _context.Products;

        public Product Get(int id)
        {
            return _context.Products
                .Include(p => p.StockProducts)
                .FirstOrDefault(p => p.Id == id);
        }

        public void Save(Product entity)
        {
            if (entity.Id == 0)
            {
                _context.Products.Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }

        public void Delete(Product entity)
        {
            _context.Products.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.StockProducts)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.StockProducts)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> SaveAsync(Product entity)
        {
            if (entity.Id == 0)
            {
                _context.Products.Add(entity);
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
            var product = await _context.Products
                .Include(p => p.StockProducts)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}