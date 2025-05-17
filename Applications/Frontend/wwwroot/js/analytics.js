// Глобальные переменные
let currentTab = 'dashboard';
window.dateRangePicker = null; // Делаем глобально доступным

// Конфигурация вкладок
const tabConfig = {
    dashboard: {
        viewHandler: 'DashboardView',
        initFunction: 'initDashboardCharts',
        destroyFunction: 'destroyDashboardCharts'
    },
    sales: {
        viewHandler: 'SalesView',
        initFunction: 'initSalesCharts',
        destroyFunction: 'destroySalesCharts'
    },
    orders: {
        viewHandler: 'OrdersView',
        initFunction: 'initOrdersCharts',
        destroyFunction: 'destroyOrdersCharts'
    },
    reports: {
        viewHandler: 'ReportsView',
        initFunction: 'initReports',
        destroyFunction: 'destroyReports'
    }
};

// Инициализация страницы
$(document).ready(function() {
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
    const $dateRangePicker = $('#dateRangePicker');
    
    if (!$dateRangePicker.length) {
        console.error('Элемент dateRangePicker не найден');
        return;
    }

    window.dateRangePicker = $dateRangePicker.daterangepicker({
        startDate: moment().subtract(29, 'days'),
        endDate: moment().endOf('day'),
        ranges: {
            'Сегодня': [moment().startOf('day'), moment().endOf('day')],
            'Вчера': [moment().subtract(1, 'days').startOf('day'), moment().subtract(1, 'days').endOf('day')],
            'Последние 7 дней': [moment().subtract(6, 'days').startOf('day'), moment().endOf('day')],
            'Последние 30 дней': [moment().subtract(29, 'days').startOf('day'), moment().endOf('day')],
            'Последний год': [moment().subtract(364, 'days').startOf('day'), moment().endOf('day')],
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
    });
}

// Инициализация обработчиков событий
function initEventListeners() {
    // Обработчик изменения периода
    $('#dateRangePicker').on('apply.daterangepicker', function(ev, picker) {
        loadCurrentTabData();
    });

    // Обработчик переключения вкладок
    $('.nav-tabs .nav-link').on('shown.bs.tab', function (e) {
        const tabId = $(e.target).attr('href').substring(1);
        
        // Вызываем функцию очистки для предыдущей вкладки
        const prevConfig = tabConfig[currentTab];
        if (prevConfig && typeof window[prevConfig.destroyFunction] === 'function') {
            window[prevConfig.destroyFunction]();
        }
        
        currentTab = tabId;
        loadCurrentTabData();
    });

    // Обработчик кнопки обновления
    $('#refreshData').click(function() {
        // Получаем текущую активную вкладку
        const activeTab = $('.nav-tabs .nav-link.active').attr('href').substring(1);
        if (activeTab !== currentTab) {
            // Если активная вкладка не совпадает с currentTab, обновляем currentTab
            currentTab = activeTab;
        }
        loadCurrentTabData();
    });

    // Обработчик для кнопки печати
    $('#printReport').on('click', function() {
        const activeTab = $('.nav-link.active').attr('id');
        const dateRange = $('#dateRangePicker').data('daterangepicker');
        const startDate = dateRange.startDate.format('YYYY-MM-DD');
        const endDate = dateRange.endDate.format('YYYY-MM-DD');
        
        const printUrl = `/Manager/Analytics/Print?section=${activeTab}&startDate=${startDate}&endDate=${endDate}`;
        window.open(printUrl, '_blank');
    });
}

// Загрузка данных для текущей вкладки
function loadCurrentTabData() {
    const dates = dateRangePicker.data('daterangepicker');
    if (!dates) {
        console.error('daterangepicker не инициализирован');
        return;
    }

    // Используем локальное время для начала и конца дня
    const startDate = dates.startDate.format('YYYY-MM-DD');
    const endDate = dates.endDate.format('YYYY-MM-DD');

    // Получаем конфигурацию текущей вкладки
    const config = tabConfig[currentTab];
    if (!config) {
        console.error(`Конфигурация для вкладки ${currentTab} не найдена`);
        return;
    }

    // Загрузка представления
    $.get(`?handler=${config.viewHandler}&startDate=${startDate}&endDate=${endDate}`, function(data) {
        // Очищаем содержимое перед вставкой новых данных
        $('#analytics-content').empty();
        $('#analytics-content').html(data);
        
        // Инициализация графиков/данных для текущей вкладки
        if (typeof window[config.initFunction] === 'function') {
            window[config.initFunction](startDate, endDate);
        }
    }).fail(function(xhr) {
        console.error(`Ошибка при загрузке данных ${currentTab}:`, xhr);
        notification.showError(`Ошибка при загрузке данных ${currentTab}`);
    });
}

// Вспомогательные функции
function formatCurrency(value) {
    const currencySymbol = $('#currencySymbol').val() || 'р.';
    return new Intl.NumberFormat('ru-RU', { 
        maximumFractionDigits: 0
    }).format(value) + ' ' + currencySymbol;
}
