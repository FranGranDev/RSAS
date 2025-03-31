using Application.Model.Stocks;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Repository
{
    public class EFStockProductsStore : IStockProductsStore
    {
        private readonly AppDbContext dbContext;

        public EFStockProductsStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<StockProduct> All => dbContext.StockProducts;

        public async Task<IEnumerable<StockProduct>> GetAllAsync()
        {
            return await dbContext.StockProducts.ToListAsync();
        }

        public async Task<StockProduct> GetByIdAsync(int id)
        {
            return await dbContext.StockProducts.FindAsync(id);
        }

        public async Task<StockProduct> Save(StockProduct stockProduct)
        {
            if (stockProduct.Id == default)
            {
                dbContext.StockProducts.Add(stockProduct);
            }
            else
            {
                dbContext.StockProducts.Update(stockProduct);
            }

            await dbContext.SaveChangesAsync();
            return stockProduct;
        }

        public async Task Delete(int id)
        {
            var stockProduct = await GetByIdAsync(id);
            if (stockProduct != null)
            {
                dbContext.StockProducts.Remove(stockProduct);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
