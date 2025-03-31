using Application.Model.Orders;
using Application.Model.Stocks;
using Application.Services;
using Application.ViewModel.Catalog;
using Microsoft.EntityFrameworkCore;
using static Application.Model.Orders.Order;

namespace Application.Data.Repository
{
    public class EFOrderStore : IOrderStore
    {
        public EFOrderStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        private readonly AppDbContext dbContext;



        public IQueryable<Order> All
        {
            get => dbContext.Orders;
        }
        public void Delete(Order order)
        {
            if (order.Id == default)
            {
                return;
            }
            dbContext.Entry(order).State = EntityState.Deleted;
            dbContext.SaveChanges();
        }
        public Order Get(int id)
        {
            return dbContext.Orders.FirstOrDefault(o => o.Id == id);
        }
        public void Save(Order order)
        {
            if (order == null)
            {
                return;
            }

            order.ChangeDate = DateTime.Now;
            if (order.Id == default)
            {
                dbContext.Entry(order).State = EntityState.Added;
            }
            else
            {
                dbContext.Entry(order).State = EntityState.Modified;
            }

            dbContext.SaveChanges();
        }


        public async Task<Order> CreateOrder(Order order, IEnumerable<CatalogItemViewModel> items, Delivery delivery)
        {
            order.OrderDate = DateTime.Now;
            order.ChangeDate = DateTime.Now;
            order.State = States.New;

            if (delivery != null)
            {
                order.Delivery = delivery;
                delivery.Order = order;
                dbContext.OrdersDelivery.Add(delivery);
            }

            order.Products = new List<OrderProduct>();


            foreach(CatalogItemViewModel item in items)
            {
                var product = await dbContext.Products.FindAsync(item.Id);

                if (product == null)
                    continue;

                var orderProduct = new OrderProduct()
                {
                    ProductId = product.Id,
                    Quantity = item.TakenCount,

                    ProductName = product.Name,
                    ProductDescription = product.Description,
                    ProductPrice = order.Type == Model.Sales.SaleTypes.Retail ? product.RetailPrice : product.WholesalePrice,
                };

                order.Products.Add(orderProduct);
                dbContext.OrderProducts.Add(orderProduct);
            }

            return order;
        }

        public void ExecuteOrder(Order order, Stock stock)
        {
            if (order == null || stock == null)
                return;
            switch (order.State)
            {
                case States.Cancelled:
                    return;
                case States.Completed:
                    return;
                case States.InProcess:
                    return;
            }

            bool isEnough = true;
            foreach(OrderProduct product in order.Products)
            {
                int quantity = stock.StockProducts.Where(x => x.ProductId == product.ProductId)
                    .Select(x => x.Quantity)
                    .Sum();

                if(quantity < product.Quantity)
                {
                    isEnough = false;
                    break;
                }
            }
            if (!isEnough)
                return;

            foreach(OrderProduct product in order.Products)
            {
                stock.StockProducts.First(x => x.ProductId == product.ProductId).Quantity -= product.Quantity;
            }

            order.StockId = stock.Id;
            order.State = States.InProcess;

            Save(order);
        }

        public void CompleteOrder(Order order)
        {
            if (order.State != States.InProcess)
                return;
            order.State = States.Completed;

            Save(order);
        }
    }
}
