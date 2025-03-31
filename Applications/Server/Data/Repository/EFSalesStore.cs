using Application.Model.Orders;
using Application.Model.Sales;
using Application.Services;

namespace Application.Data.Repository
{
    public class EFSalesStore : ISalesStore
    {
        public EFSalesStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private readonly AppDbContext dbContext;



        public IQueryable<Sale> All => dbContext.Sales;


        public Sale Get(int id)
        {
            return dbContext.Sales.FirstOrDefault(x => x.Id == id);
        }

        public void Save(Sale sale)
        {
            if (sale == null)
                return;

            if(sale.Id == default)
            {
                dbContext.Sales.Add(sale);
            }
            else
            {
                dbContext.Entry(sale).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }

            dbContext.SaveChanges();
        }


        public void CreateSale(Order order)
        {
            if (order.State != Order.States.Completed)
                return;

            Sale sale = new Sale()
            {
                OrderId = order.Id,
                StockId = order.StockId ?? 0,
                SaleDate = DateTime.Now,
                SaleType = order.Type,                
            };

            Save(sale);
        }

        public void Delete(int id)
        {
            var sale = Get(id);
            if (sale != null)
            {
                dbContext.Sales.Remove(sale);
                dbContext.SaveChanges();
            }
        }
    }
}
