// Создаем пространство имен для дашборда
const Dashboard = {
    // Глобальные переменные для хранения экземпляров графиков
    trendChart: null,
    topProductsChart: null,

    // Инициализация при загрузке страницы
    init: function() {
        // Подписываемся на события
        this.initEventListeners();
        
        // Если мы на вкладке дашборда, инициализируем графики
        if (window.currentTab === 'dashboard') {
            const dates = $('#dateRangePicker').data('daterangepicker');
            if (dates) {
                this.initCharts(dates.startDate.format('YYYY-MM-DD'), dates.endDate.format('YYYY-MM-DD'));
            }
        }
    },

    // Инициализация обработчиков событий
    initEventListeners: function() {
        const self = this;
        
        // Обработчик изменения периода
        $('#dateRangePicker').on('apply.daterangepicker', function(ev, picker) {
            if (window.currentTab === 'dashboard') {
                self.initCharts(picker.startDate.format('YYYY-MM-DD'), picker.endDate.format('YYYY-MM-DD'));
            }
        });

        // Обработчик кнопки обновления
        $('#refreshData').on('click', function() {
            if (window.currentTab === 'dashboard') {
                const dates = $('#dateRangePicker').data('daterangepicker');
                if (dates) {
                    self.initCharts(dates.startDate.format('YYYY-MM-DD'), dates.endDate.format('YYYY-MM-DD'));
                }
            }
        });
    },

    // Инициализация графиков дашборда
    initCharts: function(startDate, endDate) {
        // Загрузка данных для всех графиков
        this.loadTrendData(startDate, endDate);
        this.loadTopProductsData(startDate, endDate);
    },

    // Очистка ресурсов при уходе со вкладки
    destroyCharts: function() {
        if (this.trendChart) {
            this.trendChart.destroy();
            this.trendChart = null;
        }
        if (this.topProductsChart) {
            this.topProductsChart.destroy();
            this.topProductsChart = null;
        }
    },

    // Загрузка данных для графика тренда
    loadTrendData: function(startDate, endDate) {
        const self = this; // Сохраняем контекст
        $.get(`?handler=TrendData&startDate=${startDate}&endDate=${endDate}&interval=1d`)
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
                self.initTrendChart(chartData);
            })
            .fail(function(xhr) {
                console.error('Ошибка при загрузке данных тренда:', xhr);
            });
    },

    // Загрузка данных для графика топ товаров
    loadTopProductsData: function(startDate, endDate) {
        const self = this; // Сохраняем контекст
        $.get(`?handler=TopProductsData&startDate=${startDate}&endDate=${endDate}`)
            .done(function(data) {
                if (!data || !Array.isArray(data)) {
                    console.error('Получены некорректные данные:', data);
                    return;
                }
                self.initTopProductsChart(data);
            })
            .fail(function(xhr) {
                console.error('Ошибка при загрузке данных топ товаров:', xhr);
            });
    },

    // Инициализация графика тренда
    initTrendChart: function(data) {
        const ctx = document.getElementById('trendChart');
        if (!ctx) {
            console.error('Элемент canvas с id="trendChart" не найден');
            return;
        }

        // Уничтожаем предыдущий график, если он существует
        if (this.trendChart) {
            this.trendChart.destroy();
        }

        this.trendChart = new Chart(ctx, {
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
    },

    // Инициализация графика топ товаров
    initTopProductsChart: function(data) {
        const ctx = document.getElementById('topProductsChart');
        if (!ctx) {
            console.error('Элемент canvas с id="topProductsChart" не найден');
            return;
        }

        // Уничтожаем предыдущий график, если он существует
        if (this.topProductsChart) {
            this.topProductsChart.destroy();
        }

        const labels = data.map(item => item.productName || item.ProductName);
        const revenueData = data.map(item => item.revenue || item.Revenue);
        const salesCountData = data.map(item => item.salesCount || item.SalesCount);

        this.topProductsChart = new Chart(ctx, {
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
    },

    // Функция форматирования валюты
    formatCurrency: function(value) {
        return new Intl.NumberFormat('ru-RU', {
            style: 'currency',
            currency: 'BYN',
            maximumFractionDigits: 0
        }).format(value);
    }
};

// Глобальная функция форматирования валюты
window.formatCurrency = function(value) {
    return new Intl.NumberFormat('ru-RU', {
        style: 'currency',
        currency: 'BYN',
        maximumFractionDigits: 0
    }).format(value);
};

// Экспортируем функции для использования в analytics.js
window.initDashboardCharts = function(startDate, endDate) {
    Dashboard.initCharts(startDate, endDate);
};

window.destroyDashboardCharts = function() {
    Dashboard.destroyCharts();
};

// Метод для инициализации графиков на странице печати
window.initDashboardPrintCharts = function(data) {
    if (!data || !data.dashboard) {
        console.error('Данные для инициализации графиков отсутствуют');
        return;
    }

    // Используем существующие методы инициализации графиков
    const dashboardData = data.dashboard;

    // Инициализация графика тренда
    if (data.salesTrend && Array.isArray(data.salesTrend)) {
        const trendData = {
            labels: data.salesTrend.map(item => moment(item.date).format('DD.MM.YYYY')),
            revenue: data.salesTrend.map(item => item.revenue),
            salesCount: data.salesTrend.map(item => item.salesCount)
        };
        Dashboard.initTrendChart(trendData);
    }

    // Инициализация графика топ товаров
    if (dashboardData.topProducts && Array.isArray(dashboardData.topProducts)) {
        Dashboard.initTopProductsChart(dashboardData.topProducts);
    }
};

// --- Смена темы графиков при печати ---
(function() {
    function setLightTheme(chart) {
        if (!chart) return;
        if (chart.options.scales.x?.ticks) chart.options.scales.x.ticks.color = '#555';
        if (chart.options.scales.y?.ticks) chart.options.scales.y.ticks.color = '#555';
        if (chart.options.scales.y1?.ticks) chart.options.scales.y1.ticks.color = '#555';
        if (chart.options.scales.x?.title) chart.options.scales.x.title.color = '#555';
        if (chart.options.scales.y?.title) chart.options.scales.y.title.color = '#555';
        if (chart.options.scales.y1?.title) chart.options.scales.y1.title.color = '#555';
        if (chart.options.plugins.legend?.labels) chart.options.plugins.legend.labels.color = '#555';
        if (chart.options.plugins.title) chart.options.plugins.title.color = '#555';
        chart.update();
    }

    window.setDashboardChartsLightTheme = function() {
        setLightTheme(Dashboard.trendChart);
        setLightTheme(Dashboard.topProductsChart);
    };
    window.onbeforeprint = function() {
        window.setDashboardChartsLightTheme();
    };
})();

// Инициализация при загрузке страницы
$(document).ready(function() {
    Dashboard.init();
}); 