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

### Frontend Технологии [UI][UX]

#### Основной стек
- ASP.NET Core Razor Pages 7.0
- Bootstrap 5.3.2 (Material Design тема)
- jQuery 3.7.1
- jQuery Validation 1.20.0
- Chart.js 4.4.1

#### UI Компоненты
- DataTables 1.13.7 (для таблиц с сортировкой и фильтрацией)
- Select2 4.1.0 (для улучшенных выпадающих списков)
- Toastr.js 2.1.4 (для уведомлений)
- Font Awesome 6.4.2 (для иконок)

#### Стилизация
- SCSS для кастомных стилей
- Bootstrap 5 с Material Design темой
- Адаптивный дизайн (mobile-first подход)
- CSS Grid и Flexbox для макетов

#### Работа с API
- Fetch API для HTTP запросов
- JWT для аутентификации
- LocalStorage для хранения токена
- Axios для HTTP клиента (опционально)

#### Оптимизация
- Bundling и Minification для CSS/JS
- Кэширование данных
- Ленивая загрузка компонентов
- Оптимизация изображений

#### Безопасность
- Валидация на стороне клиента
- Защита от XSS
- Обработка CSRF токенов
- Санитизация входных данных

#### Разработка
- Visual Studio 2022
- Chrome DevTools
- ESLint для линтинга
- Prettier для форматирования кода

### Testing [Тестирование]
- xUnit для unit-тестов
- Moq для мокирования
- EF Core InMemory для тестов БД

### Infrastructure [Инфраструктура]
- Docker для контейнеризации
- GitHub Actions для CI/CD
- Azure для хостинга

## Требования к Frontend [UI][UX]

### Структура проекта
```
Frontend/
├── Pages/
│   ├── Account/
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   └── Profile.cshtml
│   ├── Clients/
│   │   ├── Index.cshtml (список клиентов)
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── Details.cshtml
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── _LoginPartial.cshtml
│       └── _ValidationScriptsPartial.cshtml
├── Services/
│   ├── ApiService.cs
│   ├── AuthService.cs
│   └── ClientService.cs
├── Models/
│   └── ViewModels/
│       ├── LoginViewModel.cs
│       ├── RegisterViewModel.cs
│       └── ClientViewModel.cs
└── wwwroot/
    ├── css/
    │   └── site.css
    ├── js/
    │   └── site.js
    └── lib/
        ├── bootstrap/
        ├── jquery/
        └── jquery-validation/
```

### Функциональные требования

#### Аутентификация [Безопасность]
- Страница входа (Login)
- Страница регистрации (Register)
- Страница профиля пользователя (Profile)
- Хранение JWT токена
- Обработка истечения токена

#### Модуль клиентов [UI]
- Список клиентов (только для менеджеров)
- Создание клиента
- Редактирование клиента
- Просмотр деталей клиента
- Поиск по имени/телефону

### Нефункциональные требования

#### UI/UX [Дизайн]
- Современный дизайн в стиле интернет-магазина
- Адаптивный дизайн для всех устройств
- Валидация форм на стороне клиента
- Уведомления об успешных/неуспешных операциях
- Загрузка данных через AJAX
- Кэширование часто используемых данных

#### Безопасность [Критично]
- Хранение JWT в localStorage
- Защита от XSS
- Валидация всех входных данных
- Обработка ошибок API

#### Производительность [Оптимизация]
- Минимальное время отклика
- Оптимизация загрузки страниц
- Кэширование данных
- Ленивая загрузка компонентов

### Технические детали

#### Модели данных [БД]
```csharp
// AppUser (Identity)
public class AppUser : IdentityUser
{
    public virtual Client Client { get; set; }
    public virtual Company Company { get; set; }
    public virtual Employee Employee { get; set; }
}

// Client
public class Client
{
    [Key] public string UserId { get; set; }
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    [Required] public string Phone { get; set; }
    public virtual AppUser User { get; set; }
}
```

#### DTO [API]
```csharp
// ClientDto
public class ClientDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}

// CreateClientDto
public class CreateClientDto
{
    [Required(ErrorMessage = "Имя клиента обязательно")]
    [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Фамилия клиента обязательна")]
    [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email клиента обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Телефон клиента обязателен")]
    [Phone(ErrorMessage = "Некорректный формат телефона")]
    public string Phone { get; set; }
}
```

#### API Endpoints [API]
```
Auth
├── POST /api/auth/login
├── POST /api/auth/register
├── POST /api/auth/forgot-password
└── POST /api/auth/reset-password

Clients
├── GET /api/clients (только для менеджеров)
├── GET /api/clients/{id}
├── POST /api/clients
├── PUT /api/clients/{id}
├── DELETE /api/clients/{id}
├── GET /api/clients/by-phone/{phone}
└── GET /api/clients/by-name?firstName={firstName}&lastName={lastName}
```

### Валидация [Безопасность]
- Email: обязательный, формат email
- Phone: обязательный, формат телефона
- FirstName: обязательный, максимум 50 символов
- LastName: обязательный, максимум 50 символов

### Роли [Безопасность]
- Client (пользователь)
- Manager (менеджер)

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