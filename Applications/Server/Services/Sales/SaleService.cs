using Application.DTOs;
using Application.Exceptions;
using Application.Models;
using AutoMapper;
using Server.Models;
using Server.Services.Repository;

namespace Server.Services.Sales
{
    public class SaleService : ISaleService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<SaleService> _logger;

        public SaleService(
            ISaleRepository saleRepository,
            IOrderRepository orderRepository,
            IMapper mapper,
            ILogger<SaleService> logger)
        {
            _saleRepository = saleRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SaleDto> CreateFromOrderAsync(int orderId)
        {
            try
            {
                _logger.LogInformation($"Начало создания продажи для заказа {orderId}");

                // Получаем заказ с деталями
                var order = await _orderRepository.GetWithDetailsAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning($"Заказ с ID {orderId} не найден");
                    throw new BusinessException($"Заказ с ID {orderId} не найден");
                }

                if (order.State != Order.States.InProcess)
                {
                    _logger.LogWarning($"Невозможно создать продажу для заказа {orderId} в статусе {order.State}");
                    throw new BusinessException("Можно создать продажу только для заказа в процессе");
                }

                // Проверяем, не существует ли уже продажа для этого заказа
                if (await _saleRepository.ExistsByOrderIdAsync(orderId))
                {
                    _logger.LogWarning($"Продажа для заказа с ID {orderId} уже существует");
                    throw new BusinessException($"Продажа для заказа с ID {orderId} уже существует");
                }

                if (order.Products == null || !order.Products.Any())
                {
                    _logger.LogWarning($"Заказ {orderId} не содержит продуктов");
                    throw new BusinessException("Заказ не содержит продуктов");
                }

                // Рассчитываем общую сумму и скидку
                decimal totalAmount = 0;
                decimal discountAmount = 0;

                // Создаем продукты продажи и считаем суммы
                var products = new List<SaleProduct>();
                foreach (var orderProduct in order.Products)
                {
                    if (orderProduct.Quantity <= 0)
                    {
                        _logger.LogWarning($"Некорректное количество продукта {orderProduct.ProductId} в заказе {orderId}");
                        throw new BusinessException($"Некорректное количество продукта {orderProduct.ProductId}");
                    }

                    if (orderProduct.ProductPrice < 0)
                    {
                        _logger.LogWarning($"Некорректная цена продукта {orderProduct.ProductId} в заказе {orderId}");
                        throw new BusinessException($"Некорректная цена продукта {orderProduct.ProductId}");
                    }

                    var productTotal = orderProduct.Quantity * orderProduct.ProductPrice;
                    totalAmount += productTotal;

                    var saleProduct = new SaleProduct
                    {
                        ProductId = orderProduct.ProductId,
                        ProductName = orderProduct.ProductName,
                        ProductCategory = orderProduct.Product.Category,
                        Quantity = orderProduct.Quantity,
                        ProductPrice = orderProduct.ProductPrice,
                        DiscountAmount = 0 // Скидка на уровне продукта не используется в заказе
                    };
                    products.Add(saleProduct);
                }

                _logger.LogInformation($"Создание продажи для заказа {orderId} с общей суммой {totalAmount}");

                // Создаем продажу
                var sale = new Sale
                {
                    OrderId = orderId,
                    ClientName = order.ClientName,
                    ClientPhone = order.ContactPhone,
                    SaleDate = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    DiscountAmount = discountAmount,
                    PaymentMethod = order.PaymentType.ToString(),
                    Comment = $"Продажа по заказу #{orderId}",
                    Products = products
                };

                // Добавляем продажу в репозиторий
                await _saleRepository.AddAsync(sale);
                await _saleRepository.SaveChangesAsync();

                // Обновляем статус заказа
                order.State = Order.States.Completed;
                await _orderRepository.UpdateAsync(order);
                await _orderRepository.SaveChangesAsync();

                _logger.LogInformation($"Продажа {sale.Id} успешно создана для заказа {orderId}");

                // Маппим Sale в SaleDto
                return new SaleDto
                {
                    Id = sale.Id,
                    OrderId = sale.OrderId,
                    ClientName = sale.ClientName,
                    ClientPhone = sale.ClientPhone,
                    SaleDate = sale.SaleDate,
                    TotalAmount = sale.TotalAmount,
                    Products = sale.Products.Select(p => new SaleProductDto
                    {
                        Id = p.Id,
                        SaleId = p.SaleId,
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        ProductCategory = p.ProductCategory,
                        Quantity = p.Quantity,
                        ProductPrice = p.ProductPrice,
                        DiscountAmount = p.DiscountAmount
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при создании продажи для заказа {orderId}");
                throw;
            }
        }
        
        public async Task<SaleDto> GetByIdAsync(int id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null)
            {
                throw new BusinessException($"Продажа с ID {id} не найдена");
            }
            return _mapper.Map<SaleDto>(sale);
        }

        public async Task<IEnumerable<SaleDto>> GetAllAsync()
        {
            var sales = await _saleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task DeleteAsync(int id)
        {
            var sale = await _saleRepository.GetWithDetailsAsync(id);
            if (sale == null)
            {
                throw new BusinessException($"Продажа с ID {id} не найдена");
            }

            await _saleRepository.DeleteAsync(sale);
        }

        public async Task<SaleDto> GetWithDetailsAsync(int id)
        {
            var sale = await _saleRepository.GetWithDetailsAsync(id);
            if (sale == null)
            {
                throw new BusinessException($"Продажа с ID {id} не найдена");
            }
            return _mapper.Map<SaleDto>(sale);
        }

        public async Task<IEnumerable<SaleDto>> GetAllWithDetailsAsync()
        {
            var sales = await _saleRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetByOrderIdAsync(int orderId)
        {
            var sales = await _saleRepository.GetByOrderIdAsync(orderId);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }
        
        public async Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sales = await _saleRepository.GetByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetByClientAsync(string clientPhone)
        {
            var sales = await _saleRepository.GetByClientAsync(clientPhone);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetByProductAsync(int productId)
        {
            var sales = await _saleRepository.GetByProductAsync(productId);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<IEnumerable<SaleDto>> GetByCategoryAsync(string category)
        {
            var sales = await _saleRepository.GetByCategoryAsync(category);
            return _mapper.Map<IEnumerable<SaleDto>>(sales);
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _saleRepository.GetTotalRevenueAsync(startDate, endDate);
        }

        public async Task<decimal> GetTotalCostAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _saleRepository.GetTotalCostAsync(startDate, endDate);
        }

        public async Task<int> GetTotalSalesCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _saleRepository.GetTotalSalesCountAsync(startDate, endDate);
        }

        public async Task<decimal> GetAverageSaleAmountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _saleRepository.GetAverageSaleAmountAsync(startDate, endDate);
        }

        public async Task<IEnumerable<TopProductDto>> GetTopProductsAsync(
            int count = 10,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var result = await _saleRepository.GetTopProductsAsync(count, startDate, endDate);
            return result.Select(x => new TopProductDto
            {
                ProductName = x.ProductName,
                SalesCount = x.SalesCount,
                Revenue = x.Revenue
            });
        }

        public async Task<IEnumerable<CategorySalesDto>> GetCategorySalesAsync(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var result = await _saleRepository.GetCategorySalesAsync(startDate, endDate);
            return result.Select(x => new CategorySalesDto
            {
                Category = x.Category,
                SalesCount = x.SalesCount,
                Revenue = x.Revenue,
                Share = x.Share
            });
        }

        public async Task<IEnumerable<SalesTrendDto>> GetSalesTrendAsync(
            DateTime startDate,
            DateTime endDate,
            TimeSpan interval)
        {
            if (startDate > endDate)
            {
                throw new BusinessException("Дата начала не может быть позже даты окончания");
            }

            if (interval <= TimeSpan.Zero)
            {
                throw new BusinessException("Интервал должен быть положительным");
            }

            var result = await _saleRepository.GetSalesTrendAsync(startDate, endDate, interval);
            
            if (!result.Any())
            {
                throw new BusinessException("Нет данных за указанный период");
            }

            return result.Select(x => new SalesTrendDto
            {
                Date = x.Date,
                Revenue = x.Revenue,
                SalesCount = x.SalesCount
            });
        }

        public async Task<bool> ExistsByOrderIdAsync(int orderId)
        {
            return await _saleRepository.ExistsByOrderIdAsync(orderId);
        }

        public async Task<DashboardAnalyticsDto> GetDashboardAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var totalRevenue = await GetTotalRevenueAsync(startDate, endDate);
            var totalSalesCount = await GetTotalSalesCountAsync(startDate, endDate);
            var averageOrderAmount = await GetAverageSaleAmountAsync(startDate, endDate);
            var topProducts = await GetTopProductsAsync(5, startDate, endDate);

            return new DashboardAnalyticsDto
            {
                TotalRevenue = totalRevenue,
                TotalSalesCount = totalSalesCount,
                AverageOrderAmount = averageOrderAmount,
                TopProducts = topProducts.ToList()
            };
        }

        public async Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var revenue = await GetTotalRevenueAsync(startDate, endDate);
            var salesCount = await GetTotalSalesCountAsync(startDate, endDate);
            var averageSaleAmount = await GetAverageSaleAmountAsync(startDate, endDate);
            var categorySales = await GetCategorySalesAsync(startDate, endDate);
            var salesTrend = await GetSalesTrendAsync(
                startDate ?? DateTime.UtcNow.AddDays(-30),
                endDate ?? DateTime.UtcNow,
                TimeSpan.FromDays(1));

            return new SalesAnalyticsDto
            {
                Period = $"{startDate?.ToString("dd.MM.yyyy") ?? "Начало"} - {endDate?.ToString("dd.MM.yyyy") ?? "Конец"}",
                Revenue = revenue,
                SalesCount = salesCount,
                AverageSaleAmount = averageSaleAmount,
                CategorySales = categorySales.ToList(),
                SalesTrend = salesTrend.ToList()
            };
        }

        public async Task<OrdersAnalyticsDto> GetOrdersAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Начало анализа заказов");

                // Устанавливаем период по умолчанию
                startDate ??= DateTime.UtcNow.AddDays(-30);
                endDate ??= DateTime.UtcNow;

                if (startDate > endDate)
                {
                    throw new BusinessException("Дата начала не может быть позже даты окончания");
                }

                // Получаем все заказы за период
                var orders = await _orderRepository.GetByDateRangeAsync(startDate.Value, endDate.Value);
                if (!orders.Any())
                {
                    return new OrdersAnalyticsDto
                    {
                        Period = $"{startDate.Value:dd.MM.yyyy} - {endDate.Value:dd.MM.yyyy}",
                        OrdersCount = 0,
                        AverageOrderAmount = 0,
                        AverageProcessingTime = TimeSpan.Zero,
                        StatusStats = new OrderStatusStatsDto(),
                        CancellationReasons = new List<CancellationReasonDto>()
                    };
                }

                // Рассчитываем статистику по статусам
                var statusStats = new OrderStatusStatsDto
                {
                    NewCount = orders.Count(o => o.State == Order.States.New),
                    ProcessingCount = orders.Count(o => o.State == Order.States.InProcess),
                    CompletedCount = orders.Count(o => o.State == Order.States.Completed),
                    CancelledCount = orders.Count(o => o.State == Order.States.Cancelled)
                };

                // Рассчитываем средний чек
                var totalAmount = orders.Sum(o => o.Products.Sum(p => p.Quantity * p.ProductPrice));
                var averageOrderAmount = orders.Any() ? totalAmount / orders.Count() : 0;

                // Рассчитываем среднее время обработки
                var completedOrders = orders.Where(o => o.State == Order.States.Completed).ToList();
                var averageProcessingTime = completedOrders.Any()
                    ? TimeSpan.FromTicks((long)completedOrders.Average(o => (o.ChangeDate - o.OrderDate).Ticks))
                    : TimeSpan.Zero;

                _logger.LogInformation("Анализ заказов успешно завершен");

                return new OrdersAnalyticsDto
                {
                    Period = $"{startDate.Value:dd.MM.yyyy} - {endDate.Value:dd.MM.yyyy}",
                    OrdersCount = orders.Count(),
                    AverageOrderAmount = averageOrderAmount,
                    AverageProcessingTime = averageProcessingTime,
                    StatusStats = statusStats,
                    CancellationReasons = new List<CancellationReasonDto>() // Пустой список, так как причины отмен не хранятся
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при анализе заказов");
                throw;
            }
        }

        public async Task<ReportDto> GenerateReportAsync(ReportType type, ReportFormat format, DateTime? startDate = null, DateTime? endDate = null)
        {
            var report = new ReportDto
            {
                Type = type,
                Format = format,
                Period = $"{startDate?.ToString("dd.MM.yyyy") ?? "Начало"} - {endDate?.ToString("dd.MM.yyyy") ?? "Конец"}"
            };

            switch (type)
            {
                case ReportType.Sales:
                    var salesAnalytics = await GetSalesAnalyticsAsync(startDate, endDate);
                    report.Data = salesAnalytics;
                    break;

                case ReportType.Orders:
                    var ordersAnalytics = await GetOrdersAnalyticsAsync(startDate, endDate);
                    report.Data = ordersAnalytics;
                    break;

                case ReportType.KPI:
                    var dashboardAnalytics = await GetDashboardAnalyticsAsync(startDate, endDate);
                    report.Data = dashboardAnalytics;
                    break;

                default:
                    throw new BusinessException($"Неподдерживаемый тип отчета: {type}");
            }

            return report;
        }
    }
}