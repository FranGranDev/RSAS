# Техническое описание проекта

## Архитектура системы [Архитектура]

### Data Models [БД]
```
Sales
├── Order [1:1]
└── Products [1:N]
    └── Product [1:1]

Orders
├── Products [1:N]
│   └── Product [1:1]
└── Delivery [1:1]

Products
└── StockProducts [1:N]
    └── Stock [1:1]

Stocks
└── Products [1:N]
    └── Product [1:1]

Users
└── Orders [1:N]
```

### Service Layer [API]
```
ISalesStore [Интерфейс]
└── SalesService [Реализация]

IOrdersStore [Интерфейс]
└── OrdersService [Реализация]

IProductsStore [Интерфейс]
└── ProductsService [Реализация]

IStocksStore [Интерфейс]
└── StocksService [Реализация]

IAuthService [Интерфейс]
└── AuthService [Реализация]
```

### API Endpoints [API]
```
Auth [Безопасность]
├── POST /api/auth/login
├── POST /api/auth/register
└── GET /api/auth/me

Orders [API]
├── GET /api/orders
├── GET /api/orders/{id}
├── POST /api/orders
├── PUT /api/orders/{id}
├── DELETE /api/orders/{id}
├── GET /api/orders/user/{userId}
└── PUT /api/orders/{orderId}/delivery

Products [API]
├── GET /api/products
├── GET /api/products/{id}
├── POST /api/products
├── PUT /api/products/{id}
├── DELETE /api/products/{id}
├── GET /api/products/name/{name}
├── GET /api/products/barcode/{barcode}
├── GET /api/products/category/{category}
└── GET /api/products/price-range

Sales [API]
├── GET /api/sales
├── GET /api/sales/{id}
├── POST /api/sales
├── PUT /api/sales/{id}
├── DELETE /api/sales/{id}
├── GET /api/sales/order/{orderId}
├── GET /api/sales/stock/{stockId}
├── GET /api/sales/status/{status}
├── GET /api/sales/date-range
├── POST /api/sales/{id}/complete
└── POST /api/sales/{id}/cancel

Stocks [API]
├── GET /api/stocks
├── GET /api/stocks/{id}
├── POST /api/stocks
├── PUT /api/stocks/{id}
├── DELETE /api/stocks/{id}
├── GET /api/stocks/name/{name}
├── GET /api/stocks/address/{address}
├── GET /api/stocks/city/{city}
├── GET /api/stocks/{stockId}/products
├── PUT /api/stocks/{stockId}/products/{productId}/quantity
├── POST /api/stocks/{stockId}/products/{productId}
└── DELETE /api/stocks/{stockId}/products/{productId}
```

### Analytics [Аналитика]
```
Sales Analytics [Аналитика]
├── Total Sales
├── Sales by Date
├── Sales by Product
├── Sales by Category
└── Sales by Stock

Orders Analytics [Аналитика]
├── Total Orders
├── Orders by Status
├── Orders by Date
└── Orders by User
```

### Security [Безопасность]
```
Authentication [Безопасность]
├── JWT Tokens
├── Password Hashing
└── Token Refresh

Authorization [Безопасность]
├── Role-Based Access
├── Policy-Based Access
└── Resource-Based Access
```

## Внешние зависимости [Зависимости]

### Database [БД]
- SQL Server 2019+
- Entity Framework Core 7.0
- Dapper для оптимизированных запросов

### ORM [БД]
- Entity Framework Core
- Fluent API для конфигурации
- Миграции для управления схемой

## Технический стек [Технологии]

### Backend [API]
- ASP.NET Core 7.0
- Entity Framework Core 7.0
- AutoMapper для маппинга DTO
- FluentValidation для валидации
- JWT для аутентификации

### Database [БД]
- SQL Server 2019
- Entity Framework Core
- Dapper для оптимизированных запросов

### Frontend [UI]
- ASP.NET Core Razor Pages
- Bootstrap 5
- jQuery для AJAX
- Chart.js для графиков

### Testing [Тестирование]
- xUnit для unit-тестов
- Moq для мокирования
- EF Core InMemory для тестов БД

### Infrastructure [Инфраструктура]
- Docker для контейнеризации
- GitHub Actions для CI/CD
- Azure для хостинга

## Структура проекта [Архитектура]

### Backend (Server) [API]
```
Server/
├── Controllers/ [API]
├── Models/ [БД]
├── Services/ [API]
├── Repositories/ [БД]
├── DTOs/ [API]
├── Middleware/ [API]
└── Data/ [БД]
```

### Frontend [UI]
```
Frontend/
├── Pages/ [UI]
├── Models/ [UI]
├── Services/ [API]
├── Components/ [UI]
└── wwwroot/ [UI]
```

## Критические зависимости [Критично]

### Security [Безопасность]
- JWT аутентификация
- Ролевая модель доступа
- Валидация входных данных

### Data [БД]
- Entity Framework Core
- SQL Server
- Миграции

### Logging [Мониторинг]
- Serilog
- Seq для анализа логов
- Elasticsearch для хранения

## Технический долг [Оптимизация]

### High Priority [Срочно]
- [Оптимизация] Оптимизация запросов к БД
- [Тестирование] Добавление unit-тестов
- [Безопасность] Улучшение валидации

### Medium Priority
- [UI] Улучшение интерфейса
- [API] Рефакторинг контроллеров
- [Документация] Обновление API docs

### Low Priority
- [Оптимизация] Кэширование
- [UI] Анимации
- [Документация] README 

## Важные файлы проекта [Архитектура]

### Конфигурация
- `Server/Program.cs` - точка входа и конфигурация приложения
- `Server/appsettings.json` - основные настройки приложения
- `Server/appsettings.Development.json` - настройки для разработки
- `Server/appsettings.Testing.json` - настройки для тестирования

### Модели данных [БД]
- `Server/Models/Sales/Sale.cs` - модель продажи
- `Server/Models/Sales/SaleProduct.cs` - модель товара в продаже
- `Server/Models/Orders/Order.cs` - модель заказа
- `Server/Models/Orders/OrderProduct.cs` - модель товара в заказе
- `Server/Models/Products/Product.cs` - модель товара
- `Server/Models/Stocks/Stock.cs` - модель склада
- `Server/Models/Stocks/StockProduct.cs` - модель товара на складе
- `Server/Models/Users/AppUser.cs` - модель пользователя
- `Server/Models/Users/Client.cs` - модель клиента
- `Server/Models/Users/Employee.cs` - модель сотрудника

### Контроллеры [API]
- `Server/Controllers/AuthController.cs` - аутентификация и авторизация
- `Server/Controllers/SalesController.cs` - управление продажами
- `Server/Controllers/OrdersController.cs` - управление заказами
- `Server/Controllers/ProductsController.cs` - управление товарами
- `Server/Controllers/StocksController.cs` - управление складами
- `Server/Controllers/ClientsController.cs` - управление клиентами
- `Server/Controllers/EmployeesController.cs` - управление сотрудниками

### Контекст базы данных [БД]
- `Server/Data/AppDbContext.cs` - контекст базы данных
- `Server/Migrations/` - миграции базы данных
- `Server/dbgeneration.sql` - скрипт генерации базы данных

### Сервисы [API]
- `Server/Services/ISalesStore.cs` - интерфейс сервиса продаж
- `Server/Services/SalesService.cs` - реализация сервиса продаж
- `Server/Services/IOrdersStore.cs` - интерфейс сервиса заказов
- `Server/Services/OrdersService.cs` - реализация сервиса заказов
- `Server/Services/IProductsStore.cs` - интерфейс сервиса товаров
- `Server/Services/ProductsService.cs` - реализация сервиса товаров
- `Server/Services/IStocksStore.cs` - интерфейс сервиса складов
- `Server/Services/StocksService.cs` - реализация сервиса складов
- `Server/Services/IAuthService.cs` - интерфейс сервиса аутентификации
- `Server/Services/AuthService.cs` - реализация сервиса аутентификации

### DTO [API]
- `Server/DTOs/Sales/` - DTO для продаж
- `Server/DTOs/Orders/` - DTO для заказов
- `Server/DTOs/Products/` - DTO для товаров
- `Server/DTOs/Stocks/` - DTO для складов
- `Server/DTOs/Users/` - DTO для пользователей

### Middleware [API]
- `Server/Middleware/ExceptionMiddleware.cs` - обработка исключений
- `Server/Middleware/JwtMiddleware.cs` - обработка JWT токенов

### Тесты [Тестирование]
- `Server.Tests/Controllers/` - тесты контроллеров

### Документация [Документация]
- `ASSISTANT_CONTEXT.md` - общий контекст проекта
- `TECHNICAL_CONTEXT.md` - технический контекст проекта