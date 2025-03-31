using Application.Model.Stocks;
using Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Repository
{
    public class EFProductsStore : IProductsStore
    {
        public EFProductsStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private readonly AppDbContext dbContext;

        public IQueryable<Product> All
        {
            get => dbContext.Products
                .Include(x => x.StockProducts);
        }

        public void Delete(Product product)
        {
            if(product.Id == default)
            {
                return;
            }
            dbContext.Entry(product).State = EntityState.Deleted;
            dbContext.SaveChanges();
        }

        public Product Get(int id)
        {
            return dbContext.Products
                .Include(x => x.StockProducts)
                .FirstOrDefault(x => x.Id == id);
        }

        public void Save(Product product)
        {
            if (product == null)
            {
                dbContext.SaveChanges();
                return;
            }

            if (product.Id == default)
                dbContext.Entry(product).State = EntityState.Added;
            else
                dbContext.Entry(product).State = EntityState.Modified;

            dbContext.SaveChanges();
        }
    }
}
