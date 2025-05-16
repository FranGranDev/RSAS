// Единый обработчик для смены темы всех графиков при печати
(function() {
    function setLightTheme(chart) {
        if (!chart) return;
        
        // Изменение цвета текста
        if (chart.options.scales?.x?.ticks) chart.options.scales.x.ticks.color = '#555';
        if (chart.options.scales?.y?.ticks) chart.options.scales.y.ticks.color = '#555';
        if (chart.options.scales?.y1?.ticks) chart.options.scales.y1.ticks.color = '#555';
        if (chart.options.scales?.x?.title) chart.options.scales.x.title.color = '#555';
        if (chart.options.scales?.y?.title) chart.options.scales.y.title.color = '#555';
        if (chart.options.scales?.y1?.title) chart.options.scales.y1.title.color = '#555';
        if (chart.options.plugins?.legend?.labels) chart.options.plugins.legend.labels.color = '#555';
        if (chart.options.plugins?.title) chart.options.plugins.title.color = '#555';

        // Изменение цвета сетки
        if (chart.options.scales?.x?.grid) {
            chart.options.scales.x.grid.color = 'rgba(0, 0, 0, 0.1)';
            chart.options.scales.x.grid.drawBorder = false;
        }
        if (chart.options.scales?.y?.grid) {
            chart.options.scales.y.grid.color = 'rgba(0, 0, 0, 0.1)';
            chart.options.scales.y.grid.drawBorder = false;
        }
        if (chart.options.scales?.y1?.grid) {
            chart.options.scales.y1.grid.color = 'rgba(0, 0, 0, 0.1)';
            chart.options.scales.y1.grid.drawBorder = false;
        }

        chart.update();
    }

    // Функция для смены темы всех графиков
    window.setAllChartsLightTheme = function() {
        console.log('Dashboard available:', !!window.Dashboard);
        console.log('Sales available:', !!window.Sales);
        console.log('Orders available:', !!window.Orders);

        // Дашборд
        if (window.Dashboard) {
            console.log('Dashboard charts:', {
                trendChart: !!window.Dashboard.trendChart,
                topProductsChart: !!window.Dashboard.topProductsChart
            });
            setLightTheme(window.Dashboard.trendChart);
            setLightTheme(window.Dashboard.topProductsChart);
        }
        
        // Продажи
        if (window.Sales) {
            console.log('Sales charts:', {
                categorySalesChart: !!window.Sales.categorySalesChart,
                forecastChart: !!window.Sales.forecastChart,
                abcAnalysisChart: !!window.Sales.abcAnalysisChart
            });
            setLightTheme(window.Sales.categorySalesChart);
            setLightTheme(window.Sales.forecastChart);
            setLightTheme(window.Sales.abcAnalysisChart);
        }
        
        // Заказы
        if (window.Orders) {
            console.log('Orders charts:', {
                statusChart: !!window.Orders.statusChart,
                trendChart: !!window.Orders.trendChart
            });
            setLightTheme(window.Orders.statusChart);
            setLightTheme(window.Orders.trendChart);
        }

        // Устанавливаем флаг для перезагрузки
        window.needsReload = true;
    };

    // Единый обработчик для печати
    window.addEventListener('beforeprint', function() {
        window.setAllChartsLightTheme();
    });
})(); 