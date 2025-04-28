using Application.DTOs;
using Application.Models;
using AutoMapper;
using Server.Models;
using System.Linq;
using Application.Extensions;

namespace Application.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Products
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();

            // Orders
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Products.Sum(p => p.ProductPrice * p.Quantity)))
                .ForMember(dest => dest.PaymentTypeDisplay, opt => opt.MapFrom(src => src.PaymentType.GetDisplayName()))
                .ForMember(dest => dest.StateDisplay, opt => opt.MapFrom(src => src.State.GetDisplayName()));
            CreateMap<CreateOrderDto, Order>();
            CreateMap<UpdateOrderDto, Order>();
            CreateMap<OrderProduct, OrderProductDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductPrice))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.ProductPrice * src.Quantity));
            CreateMap<CreateOrderProductDto, OrderProduct>()
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Price));

            // Delivery
            CreateMap<Delivery, DeliveryDto>();
            CreateMap<CreateDeliveryDto, Delivery>();
            CreateMap<UpdateDeliveryDto, Delivery>();

            // Sales
            CreateMap<Sale, SaleDto>();

            // Stocks
            CreateMap<Stock, StockDto>();
            CreateMap<CreateStockDto, Stock>();
            CreateMap<UpdateStockDto, Stock>();
            CreateMap<StockProducts, StockProductDto>();

            // Clients
            CreateMap<Client, ClientDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId));
            CreateMap<CreateClientDto, Client>();
            CreateMap<UpdateClientDto, Client>();

            // Employees
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
            CreateMap<CreateEmployeeDto, Employee>();
            CreateMap<UpdateEmployeeDto, Employee>();

            // System Users
            CreateMap<AppUser, UserDto>();
        }
    }
}