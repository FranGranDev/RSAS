using Application.Model.Stocks;
using Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Application.Data.Repository
{
    public class EFStockProductsStore : IStockProductsStore
    {
        public EFStockProductsStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private readonly AppDbContext dbContext;

        public IQueryable<StockProducts> All
        {
            get => dbContext.StockProducts;
        }

        public void Delete(StockProducts stockProducts)
        {
            dbContext.StockProducts.Remove(stockProducts);
            dbContext.SaveChanges();
        }

        public void Save(StockProducts stockProducts)
        {
            if (stockProducts == null)
            {
                dbContext.SaveChanges();
                return;
            }

            var existing = dbContext.StockProducts.FirstOrDefault(sp => sp.StockId == stockProducts.StockId && sp.ProductId == stockProducts.ProductId);

            if (existing == null)
            {
                dbContext.StockProducts.Add(stockProducts);
            }
            else
            {
                existing.Quantity = stockProducts.Quantity;
                existing.Discount = stockProducts.Discount;
                dbContext.Entry(existing).State = EntityState.Modified;
            }

            dbContext.SaveChanges();
        }
    }
}
