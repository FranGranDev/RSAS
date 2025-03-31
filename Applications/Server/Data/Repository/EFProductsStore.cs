using Application.Model.Products;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Repository
{
    public class EFProductsStore : IProductsStore
    {
        private readonly AppDbContext dbContext;

        public EFProductsStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<Product> All => dbContext.Products;

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await dbContext.Products.ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await dbContext.Products.FindAsync(id);
        }

        public async Task<Product> Save(Product product)
        {
            if (product.Id == default)
            {
                dbContext.Products.Add(product);
            }
            else
            {
                dbContext.Products.Update(product);
            }

            await dbContext.SaveChangesAsync();
            return product;
        }

        public async Task Delete(int id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                dbContext.Products.Remove(product);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
