using Application.Areas.Identity.Data;
using Application.DTOs;
using Application.Model.Orders;
using Application.Model.Sales;
using Application.Model.Stocks;
using AutoMapper;

namespace Application.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Products
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // Orders
            CreateMap<Order, OrderDto>();
            CreateMap<CreateOrderDto, Order>();
            CreateMap<UpdateOrderDto, Order>();
            CreateMap<OrderProduct, OrderProductDto>();

            // Delivery
            CreateMap<Delivery, DeliveryDto>();
            CreateMap<CreateDeliveryDto, Delivery>();
            CreateMap<UpdateDeliveryDto, Delivery>();

            // Sales
            CreateMap<Sale, SaleDto>();
            CreateMap<CreateSaleDto, Sale>();
            CreateMap<UpdateSaleDto, Sale>();

            // Stocks
            CreateMap<Stock, StockDto>();
            CreateMap<CreateStockDto, Stock>();
            CreateMap<UpdateStockDto, Stock>();
            CreateMap<StockProducts, StockProductDto>();

            // Clients
            CreateMap<Client, ClientDto>();
            CreateMap<CreateClientDto, Client>();
            CreateMap<UpdateClientDto, Client>();

            // Employees
            CreateMap<Employee, EmployeeDto>();
            CreateMap<CreateEmployeeDto, Employee>();
            CreateMap<UpdateEmployeeDto, Employee>();
        }
    }
}