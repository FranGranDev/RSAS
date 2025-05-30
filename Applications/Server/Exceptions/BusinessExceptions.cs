namespace Application.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {
        }
    }

    public class ProductNotFoundException : BusinessException
    {
        public ProductNotFoundException(int productId)
            : base($"Товар с ID {productId} не найден")
        {
        }
    }

    public class ClientNotFoundException : BusinessException
    {
        public ClientNotFoundException(string clientId)
            : base($"Клиент с ID {clientId} не найден")
        {
        }
    }

    public class EmployeeNotFoundException : BusinessException
    {
        public EmployeeNotFoundException(string employeeId)
            : base($"Сотрудник с ID {employeeId} не найден")
        {
        }
    }

    public class OrderNotFoundException : BusinessException
    {
        public OrderNotFoundException(int orderId)
            : base($"Заказ с ID {orderId} не найден")
        {
        }
    }

    public class StockNotFoundException : BusinessException
    {
        public StockNotFoundException(int stockId)
            : base($"Склад с ID {stockId} не найден")
        {
        }
    }

    public class InsufficientStockException : BusinessException
    {
        public InsufficientStockException(int productId, int requestedQuantity, int availableQuantity)
            : base(
                $"Недостаточно товара с ID {productId}. Запрошено: {requestedQuantity}, Доступно: {availableQuantity}")
        {
        }
    }

    public class InvalidOrderStateException : BusinessException
    {
        public InvalidOrderStateException(string message) : base(message)
        {
        }
    }

    public class DeliveryNotFoundException : BusinessException
    {
        public DeliveryNotFoundException(int deliveryId)
            : base($"Доставка с ID {deliveryId} не найдена")
        {
        }
    }

    public class InvalidDateRangeException : BusinessException
    {
        public InvalidDateRangeException(string message) : base(message)
        {
        }
    }

    public class InvalidAnalyticsParametersException : BusinessException
    {
        public InvalidAnalyticsParametersException(string message) : base(message)
        {
        }
    }

    public class ReportGenerationException : BusinessException
    {
        public ReportGenerationException(string message) : base(message)
        {
        }
    }

    public class AnalyticsDataNotFoundException : BusinessException
    {
        public AnalyticsDataNotFoundException(string message) : base(message)
        {
        }
    }

    public class SaleNotFoundException : BusinessException
    {
        public SaleNotFoundException(int saleId)
            : base($"Продажа с ID {saleId} не найдена")
        {
        }
    }
}