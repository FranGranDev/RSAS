$(document).ready(function() {
    // Загрузка начальных данных при открытии страницы
    loadData();
});

function loadData() {
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;

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
    $.get(`?handler=Dashboard&startDate=${startDate}&endDate=${endDate}`, function(data) {
        $('#analytics-content').html(data);
    });
}

function loadSalesData(startDate, endDate) {
    $.get(`?handler=Sales&startDate=${startDate}&endDate=${endDate}`, function(data) {
        $('#analytics-content').html(data);
    });
}

function loadOrdersData(startDate, endDate) {
    $.get(`?handler=Orders&startDate=${startDate}&endDate=${endDate}`, function(data) {
        $('#analytics-content').html(data);
    });
}

function loadReportsData(startDate, endDate) {
    $.get(`?handler=Reports&startDate=${startDate}&endDate=${endDate}`, function(data) {
        $('#analytics-content').html(data);
    });
}

// Загрузка данных при переключении вкладок
$('.nav-tabs .nav-link').on('shown.bs.tab', function (e) {
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;
    const tabId = e.target.getAttribute('href').substring(1);

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
});
