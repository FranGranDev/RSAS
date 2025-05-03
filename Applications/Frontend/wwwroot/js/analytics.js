// Глобальные переменные
let currentTab = 'dashboard';
window.dateRangePicker = null; // Делаем глобально доступным

// Инициализация страницы
$(document).ready(function() {
    console.log('Инициализация страницы аналитики');
    
    // Устанавливаем русскую локаль для moment.js
    moment.locale('ru');

    // Инициализация daterangepicker
    initDateRangePicker();
    
    // Подписка на события
    initEventListeners();
    
    // Загрузка начальных данных
    loadCurrentTabData();
});

// Инициализация daterangepicker
function initDateRangePicker() {
    console.log('Инициализация daterangepicker');
    const $dateRangePicker = $('#dateRangePicker');
    
    if (!$dateRangePicker.length) {
        console.error('Элемент dateRangePicker не найден');
        return;
    }

    window.dateRangePicker = $dateRangePicker.daterangepicker({
        startDate: moment().subtract(29, 'days'),
        endDate: moment(),
        ranges: {
            'Сегодня': [moment(), moment()],
            'Вчера': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Последние 7 дней': [moment().subtract(6, 'days'), moment()],
            'Последние 30 дней': [moment().subtract(29, 'days'), moment()],
            'Этот месяц': [moment().startOf('month'), moment().endOf('month')],
            'Прошлый месяц': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        },
        locale: {
            format: 'DD.MM.YYYY',
            applyLabel: 'Применить',
            cancelLabel: 'Отмена',
            fromLabel: 'От',
            toLabel: 'До',
            customRangeLabel: 'Произвольный период',
            daysOfWeek: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
            monthNames: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
            firstDay: 1
        },
        alwaysShowCalendars: true,
        autoApply: false,
        opens: 'right'
    }, function(start, end) {
        console.log('Выбран период:', start.format('DD.MM.YYYY'), '-', end.format('DD.MM.YYYY'));
    });

    console.log('daterangepicker инициализирован');
}

// Инициализация обработчиков событий
function initEventListeners() {
    // Обработчик изменения периода
    $('#dateRangePicker').on('apply.daterangepicker', function(ev, picker) {
        console.log('Период изменен:', picker.startDate.format('DD.MM.YYYY'), '-', picker.endDate.format('DD.MM.YYYY'));
        loadCurrentTabData();
    });

    // Обработчик переключения вкладок
    $('.nav-tabs .nav-link').on('shown.bs.tab', function (e) {
        const tabId = $(e.target).attr('href').substring(1);
        console.log('Переключение на вкладку:', tabId);
        currentTab = tabId;
        loadCurrentTabData();
    });

    // Обработчик кнопки обновления
    $('#refreshData').click(function() {
        console.log('Обновление данных');
        loadCurrentTabData();
    });
}

// Загрузка данных для текущей вкладки
function loadCurrentTabData() {
    const dates = dateRangePicker.data('daterangepicker');
    if (!dates) {
        console.error('daterangepicker не инициализирован');
        return;
    }

    const startDate = dates.startDate.format('YYYY-MM-DD');
    const endDate = dates.endDate.format('YYYY-MM-DD');
    console.log('Загрузка данных для вкладки', currentTab, 'за период:', startDate, '-', endDate);

    // Загрузка данных в зависимости от текущей вкладки
    switch (currentTab) {
        case 'dashboard':
            loadDashboardData(startDate, endDate);
            break;
        case 'sales':
            loadSalesData(startDate, endDate);
            break;
        case 'orders':
            loadOrdersData(startDate, endDate);
            break;
        case 'reports':
            loadReportsData(startDate, endDate);
            break;
    }
}

// Функции загрузки данных для каждой вкладки
function loadDashboardData(startDate, endDate) {
    $.get(`?handler=DashboardView&startDate=${startDate}&endDate=${endDate}`, function(data) {
        // Очищаем содержимое перед вставкой новых данных
        $('#analytics-content').empty();
        $('#analytics-content').html(data);
        // Инициализация графиков дашборда
        if (typeof initDashboardCharts === 'function') {
            initDashboardCharts();
        }
    }).fail(function(xhr) {
        console.error('Ошибка при загрузке данных дашборда:', xhr);
        notification.showError('Ошибка при загрузке данных дашборда');
    });
}

function loadSalesData(startDate, endDate) {
    $.get(`?handler=SalesView&startDate=${startDate}&endDate=${endDate}`, function(data) {
        // Очищаем содержимое перед вставкой новых данных
        $('#analytics-content').empty();
        $('#analytics-content').html(data);
        // Инициализация графиков продаж
        if (typeof initSalesCharts === 'function') {
            initSalesCharts();
        }
    }).fail(function(xhr) {
        console.error('Ошибка при загрузке данных продаж:', xhr);
        notification.showError('Ошибка при загрузке данных продаж');
    });
}

function loadOrdersData(startDate, endDate) {
    $.get(`?handler=OrdersView&startDate=${startDate}&endDate=${endDate}`, function(data) {
        // Очищаем содержимое перед вставкой новых данных
        $('#analytics-content').empty();
        $('#analytics-content').html(data);
        // Инициализация графиков заказов
        if (typeof initOrdersCharts === 'function') {
            initOrdersCharts();
        }
    }).fail(function(xhr) {
        console.error('Ошибка при загрузке данных заказов:', xhr);
        notification.showError('Ошибка при загрузке данных заказов');
    });
}

function loadReportsData(startDate, endDate) {
    $.get(`?handler=ReportsView&startDate=${startDate}&endDate=${endDate}`, function(data) {
        // Очищаем содержимое перед вставкой новых данных
        $('#analytics-content').empty();
        $('#analytics-content').html(data);
        // Инициализация отчетов
        if (typeof initReports === 'function') {
            initReports();
        }
    }).fail(function(xhr) {
        console.error('Ошибка при загрузке отчетов:', xhr);
        notification.showError('Ошибка при загрузке отчетов');
    });
}

// Вспомогательные функции
function formatCurrency(value) {
    const currencySymbol = $('#currencySymbol').val() || 'р.';
    return new Intl.NumberFormat('ru-RU', { 
        maximumFractionDigits: 0
    }).format(value) + ' ' + currencySymbol;
}
