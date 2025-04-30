$(document).ready(function() {
    // Загрузка данных для графика
    loadTrendData();
});

let trendChart = null;

function loadTrendData() {
    const dates = $('#dateRangePicker').data('daterangepicker');
    const startDate = dates.startDate.format('YYYY-MM-DD');
    const endDate = dates.endDate.format('YYYY-MM-DD');

    $.get(`?handler=Trend&startDate=${startDate}&endDate=${endDate}&interval=1d`)
    .done(function(data) {
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
        console.error('Ошибка при загрузке данных для графика:', xhr);
    });
}

function formatCurrency(value) {
    const currencySymbol = $('#currencySymbol').val() || 'р.';
    return new Intl.NumberFormat('ru-RU', { 
        maximumFractionDigits: 0
    }).format(value) + ' ' + currencySymbol;
}

function initTrendChart(data) {
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
                            if (context.parsed.y !== null) {
                                if (context.datasetIndex === 0) {
                                    label += formatCurrency(context.parsed.y);
                                } else {
                                    label += context.parsed.y;
                                }
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
}

// Обновление данных при изменении периода
$('#dateRangePicker').on('apply.daterangepicker', function() {
    loadTrendData();
});

// Обновление данных при нажатии кнопки обновления
$('#refreshData').click(function() {
    loadTrendData();
}); 