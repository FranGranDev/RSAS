$(document).ready(function() {
    console.log('analytics-dashboard.js загружен');
    
    // Функция для инициализации графиков
    function initializeCharts() {
        const dates = $('#dateRangePicker').data('daterangepicker');
        if (dates) {
            console.log('daterangepicker инициализирован, загружаем данные');
            loadTrendData();
            loadTopProductsData();
        } else {
            console.log('daterangepicker еще не инициализирован, ждем...');
            setTimeout(initializeCharts, 100);
        }
    }

    // Подписываемся на событие apply.daterangepicker
    $('#dateRangePicker').on('apply.daterangepicker', function(ev, picker) {
        console.log('Событие apply.daterangepicker вызвано');
        loadTrendData();
        loadTopProductsData();
    });

    // Начинаем инициализацию
    initializeCharts();
});

// Глобальные переменные для графиков
let trendChart = null;
let topProductsChart = null;

// Инициализация графиков дашборда
function initDashboardCharts() {
    console.log('Инициализация графиков дашборда');
    
    if (!window.dateRangePicker) {
        console.error('daterangepicker не инициализирован');
        return;
    }
    
    loadTrendData();
    loadTopProductsData();
}

// Загрузка данных для графика тренда
function loadTrendData() {
    const dates = window.dateRangePicker.data('daterangepicker');
    if (!dates) {
        console.error('daterangepicker не инициализирован');
        return;
    }

    const startDate = dates.startDate.format('YYYY-MM-DD');
    const endDate = dates.endDate.format('YYYY-MM-DD');
    console.log('Загрузка данных тренда за период:', startDate, '-', endDate);

    $.get(`?handler=TrendData&startDate=${startDate}&endDate=${endDate}&interval=1d`)
    .done(function(data) {
        console.log('Получены данные тренда:', data);
        if (!data || !Array.isArray(data)) {
            console.error('Получены некорректные данные:', data);
            return;
        }

        const chartData = {
            labels: data.map(item => moment(item.date).format('DD.MM.YYYY')),
            revenue: data.map(item => item.revenue),
            salesCount: data.map(item => item.salesCount)
        };
        initTrendChart(chartData);
    })
    .fail(function(xhr) {
        console.error('Ошибка при загрузке данных тренда:', xhr);
    });
}

// Загрузка данных для графика топ товаров
function loadTopProductsData() {
    const dates = window.dateRangePicker.data('daterangepicker');
    if (!dates) {
        console.error('daterangepicker не инициализирован');
        return;
    }

    const startDate = dates.startDate.format('YYYY-MM-DD');
    const endDate = dates.endDate.format('YYYY-MM-DD');
    console.log('Загрузка данных топ товаров за период:', startDate, '-', endDate);

    $.get(`?handler=TopProductsData&startDate=${startDate}&endDate=${endDate}`)
    .done(function(data) {
        console.log('Получены данные топ товаров:', data);
        if (!data || !Array.isArray(data)) {
            console.error('Получены некорректные данные:', data);
            return;
        }
        initTopProductsChart(data);
    })
    .fail(function(xhr) {
        console.error('Ошибка при загрузке данных топ товаров:', xhr);
    });
}

function formatCurrency(value) {
    const currencySymbol = $('#currencySymbol').val() || 'р.';
    return new Intl.NumberFormat('ru-RU', { 
        maximumFractionDigits: 0
    }).format(value) + ' ' + currencySymbol;
}

// Инициализация графика тренда
function initTrendChart(data) {
    console.log('Инициализация графика тренда');
    const ctx = document.getElementById('trendChart');
    if (!ctx) {
        console.error('Элемент canvas с id="trendChart" не найден');
        return;
    }

    // Уничтожаем предыдущий график, если он существует
    if (trendChart) {
        trendChart.destroy();
    }

    trendChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.labels,
            datasets: [
                {
                    label: 'Выручка',
                    data: data.revenue,
                    borderColor: 'rgb(75, 192, 192)',
                    backgroundColor: 'rgba(75, 192, 192, 0.1)',
                    tension: 0.1,
                    fill: true,
                    yAxisID: 'y',
                    pointRadius: 0,
                    pointHoverRadius: 5,
                    borderWidth: 2
                },
                {
                    label: 'Количество продаж',
                    data: data.salesCount,
                    borderColor: 'rgb(255, 99, 132)',
                    backgroundColor: 'rgba(255, 99, 132, 0.1)',
                    tension: 0.1,
                    fill: true,
                    yAxisID: 'y1',
                    pointRadius: 0,
                    pointHoverRadius: 5,
                    borderWidth: 2
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                mode: 'index',
                intersect: false,
            },
            plugins: {
                legend: {
                    position: 'top',
                    align: 'end',
                    labels: {
                        usePointStyle: true,
                        pointStyle: 'circle',
                        padding: 20,
                        color: '#fff'
                    }
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    backgroundColor: 'rgba(255, 255, 255, 0.9)',
                    titleColor: '#000',
                    bodyColor: '#000',
                    borderColor: '#ddd',
                    borderWidth: 1,
                    padding: 10,
                    displayColors: true,
                    callbacks: {
                        label: function(context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            if (context.datasetIndex === 0) {
                                label += formatCurrency(context.parsed.y);
                            } else {
                                label += context.parsed.y;
                            }
                            return label;
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false,
                        drawBorder: false
                    },
                    ticks: {
                        maxRotation: 0,
                        autoSkip: true,
                        maxTicksLimit: 8,
                        color: '#fff'
                    }
                },
                y: {
                    type: 'linear',
                    display: true,
                    position: 'left',
                    title: {
                        display: true,
                        text: 'Выручка',
                        padding: { bottom: 10 },
                        color: '#fff'
                    },
                    grid: {
                        drawBorder: false,
                        color: 'rgba(255, 255, 255, 0.1)'
                    },
                    ticks: {
                        color: '#fff',
                        callback: function(value) {
                            return formatCurrency(value);
                        }
                    }
                },
                y1: {
                    type: 'linear',
                    display: true,
                    position: 'right',
                    title: {
                        display: true,
                        text: 'Количество продаж',
                        padding: { bottom: 10 },
                        color: '#fff'
                    },
                    grid: {
                        drawOnChartArea: false,
                        drawBorder: false
                    },
                    ticks: {
                        color: '#fff',
                        precision: 0
                    }
                }
            }
        }
    });
    console.log('График тренда инициализирован');
}

// Инициализация графика топ товаров
function initTopProductsChart(data) {
    console.log('Инициализация графика топ товаров');
    const ctx = document.getElementById('topProductsChart');
    if (!ctx) {
        console.error('Элемент canvas с id="topProductsChart" не найден');
        return;
    }

    // Уничтожаем предыдущий график, если он существует
    if (topProductsChart) {
        topProductsChart.destroy();
    }

    const labels = data.map(item => item.productName || item.ProductName);
    const revenueData = data.map(item => item.revenue || item.Revenue);
    const salesCountData = data.map(item => item.salesCount || item.SalesCount);

    topProductsChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Выручка',
                    data: revenueData,
                    backgroundColor: 'rgba(75, 192, 192, 0.7)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1,
                    yAxisID: 'y'
                },
                {
                    label: 'Количество продаж',
                    data: salesCountData,
                    backgroundColor: 'rgba(255, 99, 132, 0.7)',
                    borderColor: 'rgba(255, 99, 132, 1)',
                    borderWidth: 1,
                    yAxisID: 'y1'
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                mode: 'index',
                intersect: false,
            },
            plugins: {
                legend: {
                    position: 'top',
                    align: 'end',
                    labels: {
                        usePointStyle: true,
                        pointStyle: 'circle',
                        padding: 20,
                        color: '#fff'
                    }
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    backgroundColor: 'rgba(255, 255, 255, 0.9)',
                    titleColor: '#000',
                    bodyColor: '#000',
                    borderColor: '#ddd',
                    borderWidth: 1,
                    padding: 10,
                    displayColors: true,
                    callbacks: {
                        label: function(context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            if (context.datasetIndex === 0) {
                                label += formatCurrency(context.raw);
                            } else {
                                label += context.raw;
                            }
                            return label;
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false,
                        drawBorder: false
                    },
                    ticks: {
                        maxRotation: 0,
                        autoSkip: true,
                        maxTicksLimit: 8,
                        color: '#fff'
                    }
                },
                y: {
                    type: 'linear',
                    display: true,
                    position: 'left',
                    title: {
                        display: true,
                        text: 'Выручка',
                        padding: { bottom: 10 },
                        color: '#fff'
                    },
                    grid: {
                        drawBorder: false,
                        color: 'rgba(255, 255, 255, 0.1)'
                    },
                    ticks: {
                        color: '#fff',
                        callback: function(value) {
                            return formatCurrency(value);
                        }
                    }
                },
                y1: {
                    type: 'linear',
                    display: true,
                    position: 'right',
                    title: {
                        display: true,
                        text: 'Количество продаж',
                        padding: { bottom: 10 },
                        color: '#fff'
                    },
                    grid: {
                        drawOnChartArea: false,
                        drawBorder: false
                    },
                    ticks: {
                        color: '#fff',
                        precision: 0
                    }
                }
            }
        }
    });
    console.log('График топ товаров инициализирован');
}

// Обновление данных при изменении периода
$('#dateRangePicker').on('apply.daterangepicker', function() {
    loadTrendData();
    loadTopProductsData();
});

// Обновление данных при нажатии кнопки обновления
$('#refreshData').click(function() {
    loadTrendData();
    loadTopProductsData();
}); 