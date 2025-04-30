$(document).ready(function() {
    // Устанавливаем русскую локаль для moment.js
    moment.locale('ru');

    // Инициализация daterangepicker
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

    // Обработчик изменения периода
    $('#dateRangePicker').on('apply.daterangepicker', function(ev, picker) {
        loadData();
    });

    // Загрузка начальных данных
    loadData();
});

function loadData() {
    const dates = $('#dateRangePicker').data('daterangepicker');
    const startDate = dates.startDate.format('YYYY-MM-DD');
    const endDate = dates.endDate.format('YYYY-MM-DD');

    // Загрузка данных для активной вкладки
    const activeTab = document.querySelector('.nav-tabs .nav-link.active');
    const tabId = activeTab.getAttribute('href').substring(1);

    switch (tabId) {
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

function refreshData() {
    loadData();
}

function loadDashboardData(startDate, endDate) {
    // Загрузка KPI карточек
    $.get(`?handler=Dashboard&startDate=${startDate}&endDate=${endDate}`, function(data) {
        $('#kpi-cards-container').html($(data).find('#kpi-cards-container').html());
        $('#top-products-container').html($(data).find('#top-products-container').html());
        loadTrendData(); // Загрузка данных для графика
    }).fail(function() {
        notification.showError('Ошибка при загрузке данных дашборда');
    });
}

function loadSalesData(startDate, endDate) {
    $.get(`?handler=Sales&startDate=${startDate}&endDate=${endDate}`, function(data) {
        $('#sales').html(data);
    }).fail(function() {
        notification.showError('Ошибка при загрузке данных продаж');
    });
}

function loadOrdersData(startDate, endDate) {
    $.get(`?handler=Orders&startDate=${startDate}&endDate=${endDate}`, function(data) {
        $('#orders').html(data);
    }).fail(function() {
        notification.showError('Ошибка при загрузке данных заказов');
    });
}

function loadReportsData(startDate, endDate) {
    $.get(`?handler=Reports&startDate=${startDate}&endDate=${endDate}`, function(data) {
        $('#reports').html(data);
    }).fail(function() {
        notification.showError('Ошибка при загрузке данных отчетов');
    });
}

// Обработчик переключения вкладок
$('.nav-tabs .nav-link').on('shown.bs.tab', function (e) {
    loadData();
});

// Обработчик кнопки обновления
$('#refreshData').click(function() {
    loadData();
});
