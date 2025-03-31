using Application.Model.Stocks;
using Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Repository
{
    public class EFStockStore : IStockStore
    {
        public EFStockStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private readonly AppDbContext dbContext;

        public IQueryable<Stock> All
        {
            get => dbContext.Stocks;
        }

        public void Delete(int id)
        {
            var stock = dbContext.Stocks.FirstOrDefault(x => x.Id == id);
            if (stock != null)
            {
                dbContext.Stocks.Remove(stock);
                dbContext.SaveChanges();
            }
        }

        public Stock Get(int id)
        {
            return dbContext.Stocks
                .FirstOrDefault(x => x.Id == id);
        }

        public void Save(Stock stock)
        {
            if(stock == null)
            {
                dbContext.SaveChanges();
                return;
            }    

            if (stock.Id == default)
                dbContext.Entry(stock).State = EntityState.Added;
            else
                dbContext.Entry(stock).State = EntityState.Modified;

            dbContext.SaveChanges();
        }
    }
}
