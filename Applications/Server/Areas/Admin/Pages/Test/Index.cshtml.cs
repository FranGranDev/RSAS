using Application.Areas.Identity.Data;
using Application.Data;
using Application.Model.Orders;
using Application.Model.Sales;
using Application.Model.Stocks;
using Application.Services;
using Application.ViewModel.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using static Application.Model.Orders.Order;

namespace Application.Areas.Admin.Pages.Test
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        public IndexModel(DataManager dataManager, AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.dbContext = dbContext;
            this.dataManager = dataManager;
            this.userManager = userManager;
        }

        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext dbContext;
        private readonly DataManager dataManager;


        private List<string> randomNames = new List<string>()
        {
            "���� �������",
            "������� ������",
            "����� ��������",
            "���� ��������",
            "����� ��������",
            "������� �������",
            "������� ��������",
            "���� ��������",
            "������� ������",
            "������ �����������",
            "�������� �������",
            "������ �������",
            "����� ��������",
            "����� ��������",
            "��������� �������",
            "���������� �����",
            "���� ����������",
            "������ �������",
            "��������� ��������",
            "���� ��������",
            "��������� ������",
            "����� �����",
            "����� ��������",
            "����� �����",
            "���� ��������",
            "��������� ������",
            "������ ������",
            "��������� �������",
            "������ ��������",
            "����� �������"
        };
        private List<string> randomPhones = new List<string>()
        {
            "+375171111111",
            "+375292222222",
            "+375253333333",
            "+375334444444",
            "+375445555555",
            "+375296666666",
            "+375217777777",
            "+375338888888",
            "+375449999999",
            "+375290000000",
            "+375251111111",
            "+375332222222",
            "+375213333333",
            "+375444444444",
            "+375295555555",
            "+375256666666",
            "+375337777777",
            "+375448888888",
            "+375299999999",
            "+375210000000",
            "+375271111111",
            "+375332222222",
            "+375453333333",
            "+375294444444",
            "+375255555555",
            "+375336666666",
            "+375447777777",
            "+375298888888",
            "+375219999999",
            "+375250000000",
        };
        private List<Delivery> randomDelivery = new List<Delivery>()
        {
            new Delivery()
            {
                City = "�����",
                Street = "��. ���������",
                House = "53",
                Flat = "31",
                PostalCode = "220012",
            },
            new Delivery()
            {
                City = "������",
                Street = "��. ���������",
                House = "24",
                Flat = "12",
                PostalCode = "230000",
            },
            new Delivery()
            {
                City = "�����",
                Street = "��-� ����������",
                House = "34",
                Flat = "3",
                PostalCode = "224020",
            },
            new Delivery()
            {
                City = "������",
                Street = "��. ������",
                House = "67",
                Flat = "6",
                PostalCode = "246000",
            },
            new Delivery()
            {
                City = "�������",
                Street = "��. ����",
                House = "41",
                Flat = "45",
                PostalCode = "212030",
            },
            new Delivery()
            {
                City = "�������",
                Street = "��. ������",
                House = "12",
                Flat = "10",
                PostalCode = "210000",
            },
            new Delivery()
            {
                City = "��������",
                Street = "��. ������",
                House = "7",
                Flat = "22",
                PostalCode = "213800",
            },
            new Delivery()
            {
                City = "������",
                Street = "��. ���������",
                House = "19",
                Flat = "2",
                PostalCode = "247760",
            },
            new Delivery()
            {
                City = "����",
                Street = "��. ���������������",
                House = "31",
                Flat = "15",
                PostalCode = "211391",
            },
            new Delivery()
            {
                City = "�����",
                Street = "��. ���������",
                House = "16",
                Flat = "9",
                PostalCode = "225710",
            },
        };


        public IActionResult OnPostStocks()
        {
            if (dataManager.Stocks.All.Count() > 0)
                return Page();

            List<Stock> stocks = new List<Stock>()
            {
                new Stock
                {
                    Name = "��������� �1",
                    Location = "��. ��������� 89",
                    SaleType = Stock.Types.Retail,
                },
                new Stock
                {
                    Name = "������� �1",
                    Location = "��. ������������� 127",
                    SaleType = Stock.Types.Wholesale,
                }
            };

            foreach(Stock stock in stocks)
            {
                dataManager.Stocks.Save(stock);
            }

            return Page();
        }
        public IActionResult OnPostProducts()
        {
            if (dataManager.Products.All.Count() > 0)
                return Page();

            List<Product> products = new List<Product>()
            {
                new Product()
                {
                    Name = "��� 1��",
                    Description = "25%",
                    WholesalePrice = 5,
                    RetailPrice = 8,
                },
                new Product()
                {
                    Name = "������ 1�",
                    Description = "5%",
                    WholesalePrice = 2,
                    RetailPrice = 1,
                },
                new Product()
                {
                    Name = "������ 250��",
                    Description = "45%",
                    WholesalePrice = 2,
                    RetailPrice = 4,
                },
                new Product()
                {
                    Name = "����",
                    Description = "������",
                    WholesalePrice = 2.56m,
                    RetailPrice = 1.1m,
                },
                new Product()
                {
                    Name = "������� 1��",
                    Description = "�����������",
                    WholesalePrice = 5,
                    RetailPrice = 10,
                },
                new Product()
                {
                    Name = "���� 1�",
                    Description = "������",
                    WholesalePrice = 1.24m,
                    RetailPrice = 3,
                },
            };

            foreach(Product product in products)
            {
                dataManager.Products.Save(product);
            }

            return Page();
        }
        public IActionResult OnPostAddProductsToStocks()
        {
            IEnumerable<Stock> stocks = dbContext.Stocks;
            IEnumerable<Product> products = dbContext.Products;

            foreach(Stock stock in stocks)
            {

                foreach(Product product in products)
                {
                    int quantity = new Random(DateTime.Now.Millisecond).Next(0, 100);

                    stock.StockProducts.Add(new StockProducts { StockId = stock.Id, ProductId = product.Id, Quantity = quantity });
                }
            }

            dbContext.SaveChanges();

            return Page();
        }
        public async Task<IActionResult> OnPostSalesAsync()
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                await CreateOrders(SaleTypes.Retail, 25);
                await CreateOrders(SaleTypes.Wholesale, 25);
                
                transaction.Commit();
            }
            

            return Page();
        }
        public async Task<IActionResult> OnPostClear()
        {
            dbContext.Orders.RemoveRange(dbContext.Orders);
            dbContext.Sales.RemoveRange(dbContext.Sales);
            dbContext.OrderProducts.RemoveRange(dbContext.OrderProducts);
            dbContext.OrdersDelivery.RemoveRange(dbContext.OrdersDelivery);

            await dbContext.SaveChangesAsync();

            return Page();
        }


        private async Task CreateOrders(SaleTypes saleTypes, int count)
        {
            for (int i = 0; i < count; i++)
            {
                AppUser user;
                switch(saleTypes)
                {
                    case SaleTypes.Wholesale:
                        user = dbContext.Users.Where(x => x.Company != null).OrderBy(x => EF.Functions.Random()).Take(1).First();
                        break;
                    default:
                        user = dbContext.Users.Where(x => x.Client != null).OrderBy(x => EF.Functions.Random()).Take(1).First();
                        break;
                }

                Random random = new Random(DateTime.Now.Millisecond);

                DateTime startDate = new DateTime(2022, 1, 1);
                DateTime endDate = new DateTime(2023, 5, 3);
                TimeSpan range = endDate - startDate;
                DateTime orderDate = startDate.AddDays(random.Next(range.Days)).AddHours(random.Next(24)).AddMinutes(random.Next(60));
                DateTime deliveryDate = orderDate.AddDays(random.Next(30)).AddHours(random.Next(24)).AddMinutes(random.Next(60));

                Stock stock = dbContext.Stocks.OrderBy(x => EF.Functions.Random()).Take(1).First();

                Order order = new Order()
                {
                    UserId = user.Id,
                    StockId = stock.Id,

                    ChangeDate = orderDate,
                    OrderDate = orderDate,

                    ClientName = randomNames[random.Next(0, randomNames.Count)],
                    ContactPhone = randomPhones[random.Next(0, randomPhones.Count)],

                    PaymentType = saleTypes == SaleTypes.Wholesale ? PaymentTypes.Bank : PaymentTypes.Cash,
                };

                Delivery delivery = randomDelivery[random.Next(0, randomDelivery.Count)];
                delivery.DeliveryDate = deliveryDate;

                int productCount = random.Next(1, dbContext.Products.Count());
                var products = dbContext.Products.OrderBy(x => EF.Functions.Random()).Take(productCount);

                IEnumerable<OrderProduct> items = products.Select(x => new OrderProduct()
                {
                    ProductId = x.Id,
                    Quantity = saleTypes == SaleTypes.Retail ? random.Next(1, 10) : random.Next(10, 200),

                    ProductName = x.Name,
                    ProductDescription = x.Description,
                    ProductPrice = saleTypes == SaleTypes.Retail ? x.RetailPrice : x.WholesalePrice,
                });

                order = CreateOrder(order, items, delivery);

                dbContext.Orders.Add(order);
                await dbContext.SaveChangesAsync();

                Sale sale = new Sale()
                {
                    OrderId = order.Id,
                    StockId = order.StockId ?? 0,
                    SaleDate = deliveryDate,
                    SaleType = saleTypes,
                };

                dbContext.Sales.Add(sale);
                await dbContext.SaveChangesAsync();
            }

        }
        private Order CreateOrder(Order order, IEnumerable<OrderProduct> items, Delivery delivery)
        {
            if (delivery != null)
            {
                order.Delivery = delivery;
                delivery.Order = order;
                dbContext.OrdersDelivery.Add(delivery);
            }

            order.Products = new List<OrderProduct>();


            foreach (OrderProduct item in items)
            {
                order.Products.Add(item);
                dbContext.OrderProducts.Add(item);
            }

            return order;
        }
    }
}
