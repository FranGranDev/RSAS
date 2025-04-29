# Контекст аналитики

## Структура дашборда

### 1. Основные показатели (Dashboard)

#### Функционал
- Общая выручка
- Количество продаж
- Количество заказов
- Средний чек
- Топ товаров
- Статусы заказов

#### Необходимый API
```csharp
// Получение данных для дашборда
[HttpGet("dashboard")]
public async Task<ActionResult<DashboardAnalyticsDto>> GetDashboardAnalytics(
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)

// Получение KPI
[HttpGet("kpi")]
public async Task<ActionResult<KpiDto>> GetKpi(
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)

// Получение топа продуктов
[HttpGet("top-products")]
public async Task<ActionResult<IEnumerable<TopProductResultDto>>> GetTopProducts(
    [FromQuery] int count = 10,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)
```

### 2. Продажи (Sales)

#### Функционал
- Выручка по периодам
- Динамика продаж
- Средний чек
- Продажи по категориям
- Конверсия продаж
- Маржинальная прибыль
- Рентабельность
- Эффективность складов
- Сезонность продаж
- Прогноз продаж

#### Необходимый API
```csharp
// Получение аналитики продаж
[HttpGet("analytics")]
public async Task<ActionResult<SalesAnalyticsDto>> GetSalesAnalytics(
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)

// Получение расширенной аналитики
[HttpGet("extended")]
public async Task<ActionResult<ExtendedSalesAnalyticsDto>> GetExtendedAnalytics(
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)

// Получение продаж по категориям
[HttpGet("category-sales")]
public async Task<ActionResult<IEnumerable<CategorySalesResultDto>>> GetCategorySales(
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)

// Получение тренда продаж
[HttpGet("trend")]
public async Task<ActionResult<IEnumerable<SalesTrendResultDto>>> GetSalesTrend(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate,
    [FromQuery] string interval = "1d")

// Получение прогноза продаж
[HttpGet("forecast/demand")]
public async Task<ActionResult<IEnumerable<DemandForecastDto>>> GetDemandForecast(
    [FromQuery] int days = 30,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)

// Получение анализа сезонности
[HttpGet("seasonality")]
public async Task<ActionResult<IEnumerable<SeasonalityImpactDto>>> GetSeasonalityImpact(
    [FromQuery] int years = 3,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)
```

### 3. Заказы (Orders)

#### Функционал
- Количество заказов
- Средний чек
- Время обработки заказов
- Статусы заказов
- Причины отмен

#### Необходимый API
```csharp
// Получение аналитики заказов
[HttpGet("orders-analytics")]
public async Task<ActionResult<OrdersAnalyticsDto>> GetOrdersAnalytics(
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)

// Получение статистики по статусам
[HttpGet("orders/status-stats")]
public async Task<ActionResult<OrderStatusStatsDto>> GetOrderStatusStats(
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)

// Получение причин отмен
[HttpGet("orders/cancellation-reasons")]
public async Task<ActionResult<IEnumerable<CancellationReasonDto>>> GetCancellationReasons(
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)
```

### 4. Отчеты (Reports)

#### Функционал
- Генерация отчетов
- Настройки форматирования
- Выбор типа и формата отчета

#### Необходимый API
```csharp
// Генерация отчета
[HttpPost("report")]
public async Task<ActionResult<ReportDto>> GenerateReport(
    [FromQuery] ReportType type,
    [FromQuery] ReportFormat format,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null,
    [FromBody] ReportFormattingSettings? formattingSettings = null)

// Генерация расширенного отчета
[HttpPost("report/extended")]
public async Task<ActionResult<ReportDto>> GenerateExtendedReport(
    [FromQuery] ReportType type,
    [FromQuery] ReportFormat format,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null,
    [FromBody] ReportFormattingSettings? formattingSettings = null)
```

## Общие параметры API

### Параметры запросов
1. **Период анализа:**
   - `startDate` - начальная дата
   - `endDate` - конечная дата

2. **Дополнительные параметры:**
   - `interval` - интервал группировки данных
   - `count` - количество элементов
   - `type` - тип отчета
   - `format` - формат отчета
   - `formattingSettings` - настройки форматирования

### Валидация
- Проверка корректности дат
- Проверка на будущие даты
- Проверка на корректность периода

### Обработка ошибок
- `InvalidDateRangeException`
- `InvalidAnalyticsParametersException`

## DTO модели

### Основные DTO
```csharp
public class DashboardAnalyticsDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalSalesCount { get; set; }
    public decimal AverageOrderAmount { get; set; }
    public int TotalOrdersCount { get; set; }
    public List<TopProductResultDto> TopProducts { get; set; }
    public OrderStatusStatsDto OrderStatusStats { get; set; }
}

public class SalesAnalyticsDto
{
    public string Period { get; set; }
    public decimal Revenue { get; set; }
    public int SalesCount { get; set; }
    public decimal AverageSaleAmount { get; set; }
    public List<CategorySalesResultDto> CategorySales { get; set; }
    public List<SalesTrendResultDto> SalesTrend { get; set; }
}

public class OrdersAnalyticsDto
{
    public string Period { get; set; }
    public int OrdersCount { get; set; }
    public decimal AverageOrderAmount { get; set; }
    public TimeSpan AverageProcessingTime { get; set; }
    public OrderStatusStatsDto StatusStats { get; set; }
    public List<CancellationReasonDto> CancellationReasons { get; set; }
}

public class ReportDto
{
    public int Id { get; set; }
    public ReportType Type { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Period { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string Version { get; set; }
    public ReportFormat Format { get; set; }
    public ReportFormattingSettings FormattingSettings { get; set; }
    public ReportDataDto Data { get; set; }
}
```

## Типы отчетов и форматов

```csharp
public enum ReportType
{
    [Display(Name = "Отчет по продажам")]
    Sales,

    [Display(Name = "Отчет по заказам")]
    Orders,

    [Display(Name = "Отчет по ключевым показателям")]
    KPI
}

public enum ReportFormat
{
    [Display(Name = "Excel")]
    Excel,

    [Display(Name = "PDF")]
    PDF
}
```

## Требования к интерфейсу

### 1. Общая структура страницы
- Максимальное использование ширины экрана
- Адаптивный дизайн
- Минималистичный и современный интерфейс
- Высокая производительность при работе с данными

### 2. Технический стек
- Razor Pages
- Bootstrap (тема Quartz)
- Bootstrap Icons
- JavaScript для интерактивности

### 3. Структура представлений
```
Analytics.cshtml (Основная страница)
├── _Header.cshtml (Частичное представление)
└── _Body.cshtml (Частичное представление)
    ├── _Dashboard.cshtml (Основные показатели)
    ├── _Sales.cshtml (Продажи)
    ├── _Orders.cshtml (Заказы)
    └── _Reports.cshtml (Отчеты)
```

### 4. Header
- Название страницы с иконкой
- Выбор периода анализа:
  - Календарь для выбора дат
  - Предустановленные периоды
- Кнопка обновления данных
- Навигационные вкладки

### 5. Body
- Контейнер для частичных представлений
- Динамическая загрузка контента
- Индикаторы загрузки
- Обработка ошибок

### 6. Компоненты интерфейса

#### Основные показатели (Dashboard)
- KPI карточки с иконками
- Графики динамики
- Таблицы с данными
- Индикаторы роста/падения

#### Продажи (Sales)
- Графики продаж
- Распределение по категориям
- Таблицы детальных данных
- Фильтры и группировки

#### Заказы (Orders)
- Статистика заказов
- Графики статусов
- Причины отмен
- Временные метрики

#### Отчеты (Reports)
- Форма создания отчетов
- Настройки форматирования
- История отчетов
- Кнопки экспорта

### 7. Стилизация
- Использование компонентов Bootstrap:
  - Карточки (card)
  - Сетка (grid)
  - Таблицы (table)
  - Кнопки (button)
  - Badges
  - Модальные окна
- Адаптивные классы
- Тени и скругления
- Цветовая схема Quartz

### 8. Интерактивность
- Переключение вкладок без перезагрузки
- Динамическое обновление данных
- Интерактивные графики
- Всплывающие подсказки
- Модальные окна для деталей

### 9. Производительность
- Ленивая загрузка компонентов
- Кэширование данных
- Оптимизация запросов
- Индикаторы загрузки

### 10. Обработка ошибок
- Валидация входных данных
- Сообщения об ошибках
- Состояния загрузки
- Резервные состояния

## Пути и структура файлов

### 1. Backend
```
Shared/
└── DTOs/
    └── AnalyticsDto.cs (API модели)
```

### 2. Frontend
```
Frontend/
├── Pages/
│   ├── Manager/
│   │   └── Analytics/
│   │       └── Index.cshtml (Основная страница)
│   └── Shared/
│       └── Analytics/
│           ├── _Header.cshtml (Частичное представление)
│           ├── _Body.cshtml (Частичное представление)
│           ├── _Dashboard.cshtml (Основные показатели)
│           ├── _Sales.cshtml (Продажи)
│           ├── _Orders.cshtml (Заказы)
│           └── _Reports.cshtml (Отчеты)
├── Models/
│   └── Analytics/
│       ├── DashboardModel.cs
│       ├── SalesModel.cs
│       ├── OrdersModel.cs
│       └── ReportsModel.cs
└── wwwroot/
    └── css/
        └── analytics.css (Стили для страницы аналитики)
```

### 3. Модели частичных представлений

#### DashboardModel
```csharp
public class DashboardModel
{
    public DashboardAnalyticsDto Analytics { get; set; }
    public KpiDto Kpi { get; set; }
    public IEnumerable<TopProductResultDto> TopProducts { get; set; }
}
```

#### SalesModel
```csharp
public class SalesModel
{
    public SalesAnalyticsDto Analytics { get; set; }
    public ExtendedSalesAnalyticsDto ExtendedAnalytics { get; set; }
    public IEnumerable<CategorySalesResultDto> CategorySales { get; set; }
    public IEnumerable<SalesTrendResultDto> SalesTrend { get; set; }
}
```

#### OrdersModel
```csharp
public class OrdersModel
{
    public OrdersAnalyticsDto Analytics { get; set; }
    public OrderStatusStatsDto StatusStats { get; set; }
    public IEnumerable<CancellationReasonDto> CancellationReasons { get; set; }
}
```

#### ReportsModel
```csharp
public class ReportsModel
{
    public ReportType SelectedType { get; set; }
    public ReportFormat SelectedFormat { get; set; }
    public ReportFormattingSettings FormattingSettings { get; set; }
    public IEnumerable<ReportDto> RecentReports { get; set; }
}
```

### 4. Стили
- Основные стили в `analytics.css`
- Использование Bootstrap (Quartz)
- Кастомные стили для компонентов аналитики
- Адаптивные медиа-запросы 

## Библиотеки визуализации

### 1. Chart.js
- Основная библиотека для визуализации данных
- Подключение через CDN или npm
- Типы графиков:
  - Линейные (динамика продаж)
  - Столбчатые (сравнение показателей)
  - Круговые (распределение)
  - Комбинированные

### 2. Интеграция с Razor Pages
```html
<!-- В _Layout.cshtml -->
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script src="~/js/analytics-charts.js"></script>
```

### 3. Примеры использования

#### Линейный график (динамика продаж)
```javascript
// analytics-charts.js
function createSalesTrendChart(data) {
    const ctx = document.getElementById('salesTrendChart');
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.dates,
            datasets: [{
                label: 'Выручка',
                data: data.revenue,
                borderColor: '#0d6efd',
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: 'Динамика продаж'
                }
            }
        }
    });
}
```

#### Круговая диаграмма (распределение по категориям)
```javascript
function createCategoryDistributionChart(data) {
    const ctx = document.getElementById('categoryDistributionChart');
    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: data.categories,
            datasets: [{
                data: data.values,
                backgroundColor: [
                    '#0d6efd',
                    '#6610f2',
                    '#6f42c1',
                    '#d63384',
                    '#dc3545'
                ]
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'right',
                }
            }
        }
    });
}
```

### 4. Преимущества Chart.js
- Простота интеграции с Razor Pages
- Хорошая производительность
- Богатый функционал
- Легкая настройка под тему Quartz
- Поддержка всех необходимых типов визуализации
- Интерактивность и анимации
- Адаптивность

### 5. Альтернативные варианты
- D3.js (для сложных кастомных визуализаций)
- Highcharts (коммерческое решение)
- ApexCharts (современная альтернатива)

### 6. Рекомендации по использованию
- Использовать ленивую загрузку графиков
- Кэшировать данные для графиков
- Оптимизировать количество точек на графике
- Добавлять индикаторы загрузки
- Обеспечивать доступность для скринридеров 

## Дополнительные требования

### 1. Доступ и безопасность
- Доступ к странице аналитики только для роли "Менеджер"
- Авторизация через стандартный механизм ASP.NET Core
- Валидация всех запросов к API

### 2. Обновление данных
- Данные обновляются только при нажатии кнопки "Обновить"
- Индикатор загрузки во время обновления
- Кэширование данных на клиенте
- Обработка ошибок при обновлении

### 3. Экспорт данных
- Поддержка двух форматов:
  - Excel (через API)
  - PDF (через печать страницы)
- Кнопки экспорта в header'е
- Настройки печати для PDF:
  ```css
  @media print {
      .no-print {
          display: none;
      }
      .analytics-content {
          width: 100%;
          margin: 0;
          padding: 0;
      }
      .card {
          break-inside: avoid;
      }
  }
  ```

### 4. JavaScript для экспорта
```javascript
// Экспорт в Excel
function exportToExcel() {
    const dates = getSelectedDates();
    window.location.href = `/api/analytics/export/excel?startDate=${dates.startDate}&endDate=${dates.endDate}`;
}

// Экспорт в PDF (печать)
function exportToPdf() {
    window.print();
}
```

### 5. HTML для кнопок экспорта
```html
<div class="export-buttons">
    <button class="btn btn-outline-primary" onclick="exportToExcel()">
        <i class="bi bi-file-earmark-excel"></i>
        Excel
    </button>
    <button class="btn btn-outline-primary" onclick="exportToPdf()">
        <i class="bi bi-printer"></i>
        PDF
    </button>
</div>
```

### 6. Рекомендации по экспорту
- Оптимизировать таблицы для печати
- Скрывать ненужные элементы при печати
- Настраивать размеры и отступы
- Проверять корректность отображения графиков 

## Текущая реализация страницы аналитики

### 1. Структура страницы
- Основной контейнер с классом `container-fluid` для полной ширины
- Заголовок "Аналитика" с классом `text-primary`
- Строка управления с элементами:
  - Поля выбора даты (начальная и конечная)
  - Кнопка применения (с иконкой галочки)
  - Кнопка обновления (с иконкой стрелки по кругу)
- Навигационные вкладки с классами `nav-tabs` и `nav-fill`
- Контейнер для контента `analytics-content`

### 2. JavaScript функционал
- Отдельный файл `analytics.js` для всей логики
- Основные функции:
  - `loadData()` - загрузка данных для текущей вкладки
  - `refreshData()` - обновление данных
  - `loadDashboardData()` - загрузка данных дашборда
  - `loadSalesData()` - загрузка данных продаж
  - `loadOrdersData()` - загрузка данных заказов
  - `loadReportsData()` - загрузка данных отчетов

### 3. Взаимодействие с сервером
- Использование `IAnalyticsService` для работы с API
- Обработчики в модели страницы:
  - `OnGetDashboardAsync`
  - `OnGetSalesAsync`
  - `OnGetOrdersAsync`
  - `OnGetReportsAsync`
- Все запросы учитывают выбранный период дат

### 4. Особенности реализации
- Асинхронная загрузка данных через AJAX
- Динамическое обновление контента без перезагрузки страницы
- Единый контейнер для всех частичных представлений
- Интеграция кнопки применения с полями выбора даты
- Использование иконок вместо текста для кнопок
- Обработка ошибок и индикация загрузки

### 5. Технический стек
- Razor Pages для основного представления
- Bootstrap для стилизации
- Bootstrap Icons для иконок
- jQuery для AJAX-запросов
- Частичные представления для каждого раздела

## Базовая страница аналитики

### 1. Структура страницы (Analytics.cshtml)
```html
@page
@model AnalyticsModel
@{
    ViewData["Title"] = "Аналитика";
}

<div class="analytics-page">
    <!-- Header -->
    <div class="analytics-header bg-white shadow-sm sticky-top">
        <div class="container-fluid">
            <!-- Заголовок и основные элементы -->
            <div class="d-flex align-items-center py-2">
                <!-- Название страницы -->
                <h1 class="h3 mb-0 me-3">
                    <i class="bi bi-graph-up me-2"></i>
                    Аналитика
                </h1>

                <!-- Выбор периода -->
                <div class="input-group me-3" style="width: 300px;">
                    <span class="input-group-text">
                        <i class="bi bi-calendar"></i>
                    </span>
                    <input type="text" class="form-control" id="dateRangePicker">
                </div>

                <!-- Кнопка обновления -->
                <button class="btn btn-primary" id="refreshData">
                    <i class="bi bi-arrow-clockwise"></i>
                    Обновить
                </button>
            </div>

            <!-- Навигационные вкладки -->
            <ul class="nav nav-tabs">
                <li class="nav-item">
                    <a class="nav-link active" href="#" data-tab="dashboard">
                        <i class="bi bi-speedometer2 me-1"></i>
                        Основные показатели
                    </a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#" data-tab="sales">
                        <i class="bi bi-cart me-1"></i>
                        Продажи
                    </a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#" data-tab="orders">
                        <i class="bi bi-box me-1"></i>
                        Заказы
                    </a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#" data-tab="reports">
                        <i class="bi bi-file-earmark-text me-1"></i>
                        Отчеты
                    </a>
                </li>
            </ul>
        </div>
    </div>

    <!-- Body - контейнер для частичных представлений -->
    <div class="analytics-body">
        <div id="analytics-content">
            <!-- Здесь будет загружаться контент выбранной вкладки -->
        </div>
    </div>
</div>

@section Scripts {
    <script src="~/js/analytics.js"></script>
}
```

### 2. Стили (analytics.css)
```css
.analytics-page {
    width: 100%;
    max-width: 100%;
    margin: 0;
    padding: 0;
}

.analytics-header {
    position: sticky;
    top: 0;
    z-index: 1000;
}

.analytics-body {
    padding: 1rem;
    min-height: calc(100vh - 120px);
}

/* Стили для индикатора загрузки */
.loading-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(255, 255, 255, 0.8);
    display: none;
    justify-content: center;
    align-items: center;
    z-index: 2000;
}
```

### 3. JavaScript (analytics.js)
```javascript
$(document).ready(function() {
    // Инициализация выбора периода
    $('#dateRangePicker').daterangepicker({
        startDate: moment().subtract(29, 'days'),
        endDate: moment(),
        ranges: {
            'Сегодня': [moment(), moment()],
            'Вчера': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Последние 7 дней': [moment().subtract(6, 'days'), moment()],
            'Последние 30 дней': [moment().subtract(29, 'days'), moment()],
            'Этот месяц': [moment().startOf('month'), moment().endOf('month')],
            'Прошлый месяц': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        }
    });

    // Обработка переключения вкладок
    $('.nav-link').click(function(e) {
        e.preventDefault();
        const tab = $(this).data('tab');
        loadTabContent(tab);
    });

    // Обработка обновления данных
    $('#refreshData').click(function() {
        const dates = $('#dateRangePicker').data('daterangepicker');
        loadCurrentTab(dates.startDate, dates.endDate);
    });

    // Загрузка начальной вкладки
    loadTabContent('dashboard');
});

// Функция загрузки контента вкладки
function loadTabContent(tab) {
    showLoading();
    const dates = $('#dateRangePicker').data('daterangepicker');
    
    $.get(`/Analytics/${tab}`, {
        startDate: dates.startDate.format('YYYY-MM-DD'),
        endDate: dates.endDate.format('YYYY-MM-DD')
    })
    .done(function(response) {
        $('#analytics-content').html(response);
        updateActiveTab(tab);
    })
    .fail(function(xhr) {
        showError('Ошибка загрузки данных');
    })
    .always(function() {
        hideLoading();
    });
}

// Обновление активной вкладки
function updateActiveTab(tab) {
    $('.nav-link').removeClass('active');
    $(`.nav-link[data-tab="${tab}"]`).addClass('active');
}

// Показать/скрыть индикатор загрузки
function showLoading() {
    $('.loading-overlay').show();
}

function hideLoading() {
    $('.loading-overlay').hide();
}
```

### 4. Особенности базовой страницы

#### Header
- Фиксированное положение для удобной навигации
- Название страницы с иконкой для визуального выделения
- Выбор периода через daterangepicker с предустановленными диапазонами
- Кнопка обновления данных с индикацией состояния
- Навигационные вкладки с иконками для быстрого переключения

#### Body
- Контейнер для динамического контента
- Адаптивная высота для оптимального использования пространства
- Отступы для улучшения читаемости
- Индикатор загрузки при обновлении данных

#### Интерактивность
- Переключение вкладок без перезагрузки страницы
- Обновление данных по кнопке с визуальной индикацией
- Обработка ошибок с пользовательскими сообщениями
- Сохранение состояния выбранного периода

#### Стилизация
- Использование Bootstrap (Quartz) для единообразия
- Адаптивный дизайн для всех устройств
- Тени и скругления для визуальной глубины
- Иконки Bootstrap для улучшения UX

// ... existing code ... 