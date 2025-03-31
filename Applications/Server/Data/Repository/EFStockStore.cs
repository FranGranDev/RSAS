using Application.Model.Stocks;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Repository
{
    public class EFStockStore : IStockStore
    {
        private readonly AppDbContext dbContext;

        public EFStockStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<Stock> All => dbContext.Stocks;

        public async Task<IEnumerable<Stock>> GetAllAsync()
        {
            return await dbContext.Stocks.ToListAsync();
        }

        public async Task<Stock> GetByIdAsync(int id)
        {
            return await dbContext.Stocks.FindAsync(id);
        }

        public async Task<Stock> Save(Stock stock)
        {
            if (stock.Id == default)
            {
                dbContext.Stocks.Add(stock);
            }
            else
            {
                dbContext.Stocks.Update(stock);
            }

            await dbContext.SaveChangesAsync();
            return stock;
        }

        public async Task Delete(int id)
        {
            var stock = await GetByIdAsync(id);
            if (stock != null)
            {
                dbContext.Stocks.Remove(stock);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
