// Создаем пространство имен для заказов
const Orders = {
    // Глобальные переменные для хранения экземпляров графиков
    statusChart: null,
    trendChart: null,

    // Инициализация при загрузке страницы
    init: function() {
        // Подписываемся на события
        this.initEventListeners();
        
        // Если мы на вкладке заказов, инициализируем графики
        if (window.currentTab === 'orders') {
            const dates = $('#dateRangePicker').data('daterangepicker');
            if (dates) {
                this.initCharts(dates.startDate.utc().format('YYYY-MM-DD'), dates.endDate.utc().format('YYYY-MM-DD'));
            }
        }
    },

    // Инициализация обработчиков событий
    initEventListeners: function() {
        const self = this;
        
        // Обработчик изменения периода
        $('#dateRangePicker').on('apply.daterangepicker', function(ev, picker) {
            if (window.currentTab === 'orders') {
                self.initCharts(picker.startDate.utc().format('YYYY-MM-DD'), picker.endDate.utc().format('YYYY-MM-DD'));
            }
        });

        // Обработчик кнопки обновления
        $('#refreshData').on('click', function() {
            if (window.currentTab === 'orders') {
                const dates = $('#dateRangePicker').data('daterangepicker');
                if (dates) {
                    self.initCharts(dates.startDate.utc().format('YYYY-MM-DD'), dates.endDate.utc().format('YYYY-MM-DD'));
                }
            }
        });
    },

    // Инициализация графиков
    initCharts: function(startDate, endDate) {
        // Загрузка данных для всех графиков
        this.loadOrdersData(startDate, endDate);
        this.loadTrendData(startDate, endDate);
    },

    // Очистка ресурсов при уходе со вкладки
    destroyCharts: function() {
        if (this.statusChart) {
            this.statusChart.destroy();
            this.statusChart = null;
        }
        if (this.trendChart) {
            this.trendChart.destroy();
            this.trendChart = null;
        }
    },

    // Загрузка данных для графика статусов
    loadOrdersData: function(startDate, endDate) {
        $.get(`?handler=OrdersData&startDate=${startDate}&endDate=${endDate}`)
        .done(function(data) {
            // Обновляем KPI карточки
            $('#ordersCount').text(data.ordersCount);
            $('#averageOrderAmount').text(Orders.formatCurrency(data.averageOrderAmount));
            $('#averageProductsPerOrder').text(data.averageProductsPerOrder.toFixed(1));
            $('#averageProcessingTime').text(Orders.formatTimeSpan(data.averageProcessingTime));
            
            // Обновляем графики
            Orders.initStatusChart(data);
        })
        .fail(function(error) {
            console.error('Ошибка при загрузке данных:', error);
        });
    },

    // Загрузка данных для графика трендов
    loadTrendData: function(startDate, endDate) {
        $.get(`?handler=TrendData&startDate=${startDate}&endDate=${endDate}&interval=1d`)
            .done(function(data) {
                if (!data || !Array.isArray(data)) {
                    console.error('Получены некорректные данные:', data);
                    return;
                }
                Orders.initTrendChart(data);
            })
            .fail(function(xhr) {
                console.error('Ошибка при загрузке данных тренда:', xhr);
            });
    },

    // Инициализация графика статусов
    initStatusChart: function(data) {
        const ctx = document.getElementById('orderStatusChart');
        if (!ctx) {
            console.error('Элемент canvas с id="orderStatusChart" не найден');
            return;
        }

        // Уничтожаем предыдущий график, если он существует
        if (this.statusChart) {
            this.statusChart.destroy();
        }

        this.statusChart = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: ['Новые', 'В обработке', 'Завершенные', 'Отмененные'],
                datasets: [{
                    data: [
                        data.statusStats.newCount,
                        data.statusStats.processingCount,
                        data.statusStats.completedCount,
                        data.statusStats.cancelledCount
                    ],
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.8)',
                        'rgba(255, 206, 86, 0.8)',
                        'rgba(75, 192, 192, 0.8)',
                        'rgba(255, 99, 132, 0.8)'
                    ],
                    borderColor: [
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(255, 99, 132, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'right',
                        labels: {
                            color: '#fff'
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(255, 255, 255, 0.9)',
                        titleColor: '#000',
                        bodyColor: '#000',
                        borderColor: '#ddd',
                        borderWidth: 1,
                        padding: 10,
                        callbacks: {
                            label: function(context) {
                                const label = context.label || '';
                                const value = context.raw || 0;
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                const percentage = ((value / total) * 100).toFixed(1);
                                return `${label}: ${value} (${percentage}%)`;
                            }
                        }
                    }
                }
            }
        });
    },

    // Инициализация графика трендов
    initTrendChart: function(data) {
        const ctx = document.getElementById('orderTrendChart');
        if (!ctx) {
            console.error('Элемент canvas с id="orderTrendChart" не найден');
            return;
        }

        // Уничтожаем предыдущий график, если он существует
        if (this.trendChart) {
            this.trendChart.destroy();
        }

        this.trendChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.map(item => moment(item.date).format('DD.MM.YYYY')),
                datasets: [
                    {
                        label: 'Количество заказов',
                        data: data.map(item => item.salesCount),
                        borderColor: 'rgb(75, 192, 192)',
                        backgroundColor: 'rgba(75, 192, 192, 0.1)',
                        tension: 0.1,
                        fill: true,
                        yAxisID: 'y'
                    },
                    {
                        label: 'Средний чек',
                        data: data.map(item => item.revenue),
                        borderColor: 'rgb(255, 99, 132)',
                        backgroundColor: 'rgba(255, 99, 132, 0.1)',
                        tension: 0.1,
                        fill: true,
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
                        labels: {
                            color: '#fff'
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(255, 255, 255, 0.9)',
                        titleColor: '#000',
                        bodyColor: '#000',
                        borderColor: '#ddd',
                        borderWidth: 1,
                        padding: 10,
                        callbacks: {
                            label: function(context) {
                                let label = context.dataset.label || '';
                                if (label) {
                                    label += ': ';
                                }
                                if (context.datasetIndex === 0) {
                                    label += context.parsed.y;
                                } else {
                                    label += Orders.formatCurrency(context.parsed.y);
                                }
                                return label;
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        },
                        ticks: {
                            color: '#fff',
                            maxRotation: 45,
                            minRotation: 45
                        }
                    },
                    y: {
                        type: 'linear',
                        display: true,
                        position: 'left',
                        title: {
                            display: true,
                            text: 'Количество заказов',
                            color: '#fff'
                        },
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        },
                        ticks: {
                            color: '#fff'
                        }
                    },
                    y1: {
                        type: 'linear',
                        display: true,
                        position: 'right',
                        title: {
                            display: true,
                            text: 'Средний чек',
                            color: '#fff'
                        },
                        grid: {
                            drawOnChartArea: false
                        },
                        ticks: {
                            color: '#fff',
                            callback: function(value) {
                                return Orders.formatCurrency(value);
                            }
                        }
                    }
                }
            }
        });
    },

    // Функция форматирования валюты
    formatCurrency: function(value) {
        return new Intl.NumberFormat('ru-RU', { 
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }).format(value) + ' р.';
    },

    // Функция форматирования TimeSpan
    formatTimeSpan: function(timeSpan) {
        if (!timeSpan) return '0ч 0мин';
        
        // Проверяем, является ли timeSpan объектом TimeSpan
        if (typeof timeSpan === 'object' && timeSpan.hasOwnProperty('hours') && timeSpan.hasOwnProperty('minutes')) {
            return `${timeSpan.hours}ч ${timeSpan.minutes}мин`;
        }
        
        // Если timeSpan - строка в формате "HH:mm:ss"
        if (typeof timeSpan === 'string') {
            const parts = timeSpan.split(':');
            if (parts.length >= 2) {
                return `${parseInt(parts[0])}ч ${parseInt(parts[1])}мин`;
            }
        }
        
        return '0ч 0мин';
    }
};

// Экспортируем функции для использования в analytics.js
window.initOrdersCharts = function(startDate, endDate) {
    Orders.initCharts(startDate, endDate);
};

window.destroyOrdersCharts = function() {
    Orders.destroyCharts();
};

// Инициализация при загрузке страницы
$(document).ready(function() {
    Orders.init();
}); 