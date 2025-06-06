// Создаем пространство имен для продаж
const Sales = {
    // Глобальные переменные для хранения экземпляров графиков
    categorySalesChart: null,
    forecastChart: null,
    abcAnalysisChart: null,

    // Инициализация графиков
    initCharts: function(startDate, endDate) {
        // Загрузка данных для всех графиков
        this.loadCategorySalesData(startDate, endDate);
        this.loadForecastData(startDate, endDate);
        this.loadAbcAnalysisData(startDate, endDate);
    },

    // Очистка ресурсов при уходе со вкладки
    destroyCharts: function() {
        if (this.categorySalesChart) {
            this.categorySalesChart.destroy();
            this.categorySalesChart = null;
        }
        if (this.forecastChart) {
            this.forecastChart.destroy();
            this.forecastChart = null;
        }
        if (this.abcAnalysisChart) {
            this.abcAnalysisChart.destroy();
            this.abcAnalysisChart = null;
        }
    },

    // Загрузка данных для графика продаж по категориям
    loadCategorySalesData: function(startDate, endDate) {
        $.get(`?handler=CategorySalesData&startDate=${startDate}&endDate=${endDate}`)
            .done(function(data) {
                if (!data || !Array.isArray(data)) {
                    console.error('Получены некорректные данные:', data);
                    return;
                }
                Sales.initCategorySalesChart(data);
            })
            .fail(function(xhr) {
                console.error('Ошибка при загрузке данных продаж по категориям:', xhr);
            });
    },

    // Загрузка данных для графика прогноза
    loadForecastData: function(startDate, endDate) {
        $.get(`?handler=ForecastData&startDate=${startDate}&endDate=${endDate}`)
            .done(function(data) {
                if (!data || !Array.isArray(data)) {
                    console.error('Получены некорректные данные:', data);
                    return;
                }
                Sales.initForecastChart(data);
            })
            .fail(function(xhr) {
                console.error('Ошибка при загрузке данных прогноза:', xhr);
            });
    },

    // Загрузка данных для графика ABC-анализа
    loadAbcAnalysisData: function(startDate, endDate) {
        $.get(`?handler=AbcAnalysisData&startDate=${startDate}&endDate=${endDate}`)
            .done(function(data) {
                if (!data || !Array.isArray(data)) {
                    console.error('Получены некорректные данные:', data);
                    return;
                }
                Sales.initAbcAnalysisChart(data);
            })
            .fail(function(xhr) {
                console.error('Ошибка при загрузке данных ABC-анализа:', xhr);
            });
    },

    // Инициализация графика продаж по категориям
    initCategorySalesChart: function(data) {
        const ctx = document.getElementById('categorySalesChart');
        if (!ctx) {
            console.error('Элемент canvas с id="categorySalesChart" не найден');
            return;
        }

        // Уничтожаем предыдущий график, если он существует
        if (this.categorySalesChart) {
            this.categorySalesChart.destroy();
        }

        this.categorySalesChart = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: data.map(item => item.category),
                datasets: [{
                    data: data.map(item => item.revenue),
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.8)',
                        'rgba(54, 162, 235, 0.8)',
                        'rgba(255, 206, 86, 0.8)',
                        'rgba(75, 192, 192, 0.8)',
                        'rgba(153, 102, 255, 0.8)'
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(153, 102, 255, 1)'
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
                                const salesCount = data[context.dataIndex].salesCount;
                                return [
                                    `${label}: ${new Intl.NumberFormat('ru-RU', { 
                                        maximumFractionDigits: 0
                                    }).format(value)} р. (${percentage}%)`,
                                    `Количество продаж: ${salesCount}`
                                ];
                            }
                        }
                    }
                }
            }
        });
    },

    // Инициализация графика прогноза
    initForecastChart: function(data) {
        const ctx = document.getElementById('forecastChart');
        if (!ctx) {
            console.error('Элемент canvas с id="forecastChart" не найден');
            return;
        }

        // Уничтожаем предыдущий график, если он существует
        if (this.forecastChart) {
            this.forecastChart.destroy();
        }

        // Сохраняем данные для использования в подсказках
        const forecastData = data;

        // Подготавливаем данные для графика
        const labels = data.map(item => item.productName);
        const forecastQuantity = data.map(item => item.forecastedQuantity);
        const currentStockData = data.map(item => item.currentStock);
        const recommendedStockData = data.map(item => item.currentStock + item.recommendedOrder);
        const lowerBoundData = data.map(item => item.lowerBound);
        const upperBoundData = data.map(item => item.upperBound);
        const confidenceData = data.map(item => item.confidence);

        this.forecastChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Прогноз спроса',
                        data: forecastQuantity,
                        backgroundColor: confidenceData.map(conf => 
                            `rgba(75, 192, 192, ${0.5 + conf * 0.5})`),
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1,
                        order: 1
                    },
                    {
                        label: 'Текущий остаток',
                        data: currentStockData,
                        backgroundColor: 'rgba(54, 162, 235, 0.7)',
                        borderColor: 'rgba(54, 162, 235, 1)',
                        borderWidth: 1,
                        order: 2
                    },
                    {
                        label: 'Рекомендуемый запас',
                        data: recommendedStockData,
                        backgroundColor: 'rgba(255, 159, 64, 0.7)',
                        borderColor: 'rgba(255, 159, 64, 1)',
                        borderWidth: 1,
                        order: 3
                    },
                    {
                        label: 'Доверительный интервал',
                        data: upperBoundData,
                        backgroundColor: 'rgba(255, 99, 132, 0.2)',
                        borderColor: 'rgba(255, 99, 132, 0.5)',
                        borderWidth: 1,
                        type: 'line',
                        fill: true,
                        order: 4,
                        pointRadius: 0,
                        tension: 0.1
                    },
                    {
                        label: 'Нижняя граница',
                        data: lowerBoundData,
                        backgroundColor: 'rgba(255, 99, 132, 0.2)',
                        borderColor: 'rgba(255, 99, 132, 0.5)',
                        borderWidth: 1,
                        type: 'line',
                        fill: false,
                        order: 4,
                        pointRadius: 0,
                        tension: 0.1
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
                    title: {
                        display: true,
                        text: 'Прогноз спроса на 30 дней',
                        color: '#fff',
                        font: {
                            size: 16,
                            weight: 'bold'
                        }
                    },
                    legend: {
                        position: 'top',
                        labels: {
                            color: '#fff',
                            usePointStyle: true,
                            pointStyle: 'circle'
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
                            title: function(context) {
                                const index = context[0].dataIndex;
                                const item = forecastData[index];
                                return [
                                    item.productName,
                                    `Категория: ${item.category}`,
                                    `Прогнозируемая выручка: ${Sales.formatCurrency(item.forecastedRevenue)}`
                                ];
                            },
                            label: function(context) {
                                const index = context.dataIndex;
                                const item = forecastData[index];
                                const dataset = context.dataset;
                                const value = context.raw;
                                
                                if (dataset.label === 'Прогноз спроса') {
                                    return [
                                        `Прогнозируемый спрос: ${value} ед.`,
                                        `Доверительный интервал: ${item.lowerBound} - ${item.upperBound} ед.`,
                                        `Уверенность в прогнозе: ${(item.confidence * 100).toFixed(1)}%`
                                    ];
                                } else if (dataset.label === 'Текущий остаток') {
                                    return `Текущий остаток: ${value} ед.`;
                                } else if (dataset.label === 'Рекомендуемый запас') {
                                    return [
                                        `Рекомендуемый запас: ${value} ед.`,
                                        `(Текущий остаток + ${item.recommendedOrder} ед.)`,
                                        item.message
                                    ];
                                } else if (dataset.label === 'Доверительный интервал') {
                                    return `Верхняя граница: ${value} ед.`;
                                } else if (dataset.label === 'Нижняя граница') {
                                    return `Нижняя граница: ${value} ед.`;
                                }
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Количество, ед.',
                            color: '#fff'
                        },
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        },
                        ticks: {
                            color: '#fff',
                            callback: function(value) {
                                return value.toLocaleString('ru-RU');
                            }
                        }
                    },
                    x: {
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        },
                        ticks: {
                            color: '#fff',
                            maxRotation: 45,
                            minRotation: 45
                        }
                    }
                }
            }
        });
    },

    // Инициализация графика ABC-анализа
    initAbcAnalysisChart: function(data) {
        const ctx = document.getElementById('abcAnalysisChart').getContext('2d');
        
        // Сохраняем данные для тултипов
        const tooltipData = data;
        
        // Определяем цвета для категорий ABC
        const categoryColors = {
            'A': 'rgba(75, 192, 192, 0.8)',  // зеленый
            'B': 'rgba(255, 206, 86, 0.8)',  // желтый
            'C': 'rgba(255, 99, 132, 0.8)'   // красный
        };

        // Уничтожаем предыдущий график, если он существует
        if (this.abcAnalysisChart) {
            this.abcAnalysisChart.destroy();
        }

        // Создаем новый график и сохраняем его в объект Sales
        this.abcAnalysisChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.map(item => item.productName),
                datasets: [{
                    label: 'Доля в выручке',
                    data: data.map(item => item.share * 100),
                    backgroundColor: data.map(item => categoryColors[item.abcCategory]),
                    borderColor: data.map(item => categoryColors[item.abcCategory].replace('0.8', '1')),
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    tooltip: {
                        backgroundColor: 'rgba(255, 255, 255, 0.9)',
                        titleColor: '#000',
                        bodyColor: '#000',
                        borderColor: '#ddd',
                        borderWidth: 1,
                        padding: 10,
                        callbacks: {
                            title: function(tooltipItems) {
                                const data = tooltipData[tooltipItems[0].dataIndex];
                                return [
                                    data.productName,
                                    `Категория товара: ${data.category}`,
                                    `Группа ABC: ${data.abcCategory}`
                                ];
                            },
                            label: function(tooltipItem) {
                                const data = tooltipData[tooltipItem.dataIndex];
                                return [
                                    `Выручка: ${Sales.formatCurrency(data.revenue)}`,
                                    `Доля: ${(data.share * 100).toFixed(1)}%`,
                                    `Накопительная доля: ${(data.cumulativeShare * 100).toFixed(1)}%`
                                ];
                            }
                        }
                    },
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Доля в выручке (%)',
                            color: '#fff'
                        },
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        },
                        ticks: {
                            color: '#fff',
                            callback: function(value) {
                                return value + '%';
                            }
                        }
                    },
                    x: {
                        grid: {
                            color: 'rgba(255, 255, 255, 0.1)'
                        },
                        ticks: {
                            color: '#fff',
                            maxRotation: 45,
                            minRotation: 45
                        },
                        title: {
                            display: true,
                            text: 'Товары',
                            color: '#fff'
                        }
                    }
                }
            }
        });
    },

    // Функция форматирования валюты
    formatCurrency: function(value) {
        return new Intl.NumberFormat('ru-RU', { 
            maximumFractionDigits: 0
        }).format(value) + ' р.';
    }
};

// Делаем Sales доступным глобально
window.Sales = Sales;

// Экспортируем функции для использования в analytics.js
window.initSalesCharts = function(startDate, endDate) {
    Sales.initCharts(startDate, endDate);
};

window.destroySalesCharts = function() {
    Sales.destroyCharts();
};

// Метод для инициализации графиков на странице печати
window.initSalesPrintCharts = function(data) {
    if (!data) {
        console.error('Данные для инициализации графиков отсутствуют');
        return;
    }

    // Используем существующие методы инициализации графиков
    if (data.salesAnalytics && data.salesAnalytics.categorySales) {
        Sales.initCategorySalesChart(data.salesAnalytics.categorySales);
    }

    if (data.salesForecast) {
        Sales.initForecastChart(data.salesForecast);
    }

    if (data.abcAnalysis) {
        Sales.initAbcAnalysisChart(data.abcAnalysis);
    }
};

// Инициализация при загрузке страницы
$(document).ready(function() {
    // Если текущая вкладка - продажи, инициализируем графики
    if (window.currentTab === 'sales') {
        Sales.initCharts();
    }
});

// --- Смена темы графиков при печати ---
(function() {
    function setLightTheme(chart) {
        if (!chart) return;
        if (chart.options.scales?.x?.ticks) chart.options.scales.x.ticks.color = '#555';
        if (chart.options.scales?.y?.ticks) chart.options.scales.y.ticks.color = '#555';
        if (chart.options.scales?.y1?.ticks) chart.options.scales.y1.ticks.color = '#555';
        if (chart.options.scales?.x?.title) chart.options.scales.x.title.color = '#555';
        if (chart.options.scales?.y?.title) chart.options.scales.y.title.color = '#555';
        if (chart.options.scales?.y1?.title) chart.options.scales.y1.title.color = '#555';
        if (chart.options.plugins?.legend?.labels) chart.options.plugins.legend.labels.color = '#555';
        if (chart.options.plugins?.title) chart.options.plugins.title.color = '#555';
        chart.update();
    }

    window.setSalesChartsLightTheme = function() {
        setLightTheme(Sales.categorySalesChart);
        setLightTheme(Sales.forecastChart);
        setLightTheme(Sales.abcAnalysisChart);
    };

    // Добавляем обработчик для печати
    window.addEventListener('beforeprint', function() {
        window.setSalesChartsLightTheme();
    });
})();