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
    
    $.get(`/Manager/Analytics?handler=${tab}`, {
        startDate: dates.startDate.format('YYYY-MM-DD'),
        endDate: dates.endDate.format('YYYY-MM-DD')
    })
    .done(function(response) {
        $('#analytics-content').html(response);
        updateActiveTab(tab);
        initializeCharts();
    })
    .fail(function(xhr) {
        notification.showError('Ошибка загрузки данных');
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

// Инициализация графиков
function initializeCharts() {
    // Здесь будет код инициализации графиков
    // для каждой вкладки
} 