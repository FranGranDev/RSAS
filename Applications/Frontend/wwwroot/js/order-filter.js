$(document).ready(function () {
    const ordersGrid = $('#ordersGrid');
    const searchInput = $('#searchInput');
    const statusFilter = $('#statusFilter');
    const dateRangePicker = $('#dateRangePicker');
    const dateSort = $('#dateSort');
    const pagination = $('#pagination');
    const resetDates = $('#resetDates');
    
    let currentPage = 1;
    let itemsPerPage = 6;
    let filteredOrders = Array.from(document.querySelectorAll('.order-card'));
    
    // Инициализация daterangepicker
    dateRangePicker.daterangepicker({
        startDate: moment().subtract(364, 'days'),
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
    
    // Функция фильтрации заказов
    function filterOrders() {
        const searchTerm = searchInput.val().toLowerCase();
        const statusValue = statusFilter.val();
        const dateRange = dateRangePicker.data('daterangepicker');
        const startDate = dateRange.startDate.toDate();
        const endDate = dateRange.endDate.toDate();
        
        filteredOrders = Array.from(document.querySelectorAll('.order-card')).filter(card => {
            const id = card.dataset.id;
            const status = card.dataset.status;
            const orderDate = new Date(card.dataset.date);
            
            const matchesSearch = id.includes(searchTerm);
            const matchesStatus = !statusValue || status === statusValue.toLowerCase();
            const matchesDate = orderDate >= startDate && orderDate <= endDate;
            
            return matchesSearch && matchesStatus && matchesDate;
        });
        
        // Показываем/скрываем карточки
        $('.order-card').hide();
        filteredOrders.forEach(card => $(card).show());
        
        sortOrders();
        showCurrentPage();
        updatePagination();
    }
    
    // Функция сортировки заказов
    function sortOrders() {
        const sortValue = dateSort.val();

        if (sortValue) {
            filteredOrders.sort((a, b) => {
                const dateA = new Date(a.dataset.date);
                const dateB = new Date(b.dataset.date);
                
                if (sortValue === 'asc') {
                    return dateA - dateB;
                } else {
                    return dateB - dateA;
                }
            });

            // Обновляем порядок элементов в DOM
            const ordersGrid = document.getElementById('ordersGrid');
            filteredOrders.forEach(card => {
                ordersGrid.appendChild(card);
            });
        }
    }
    
    // Функция отображения текущей страницы
    function showCurrentPage() {
        const start = (currentPage - 1) * itemsPerPage;
        const end = start + itemsPerPage;
        
        $('.order-card').hide();
        filteredOrders.slice(start, end).forEach(card => $(card).show());
    }
    
    // Функция обновления пагинации
    function updatePagination() {
        const totalPages = Math.ceil(filteredOrders.length / itemsPerPage);
        let paginationHtml = '';
        
        if (totalPages > 1) {
            const maxVisiblePages = 5; // Максимальное количество видимых страниц
            let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2));
            let endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);
            
            // Корректируем startPage, если endPage достиг конца
            if (endPage - startPage + 1 < maxVisiblePages) {
                startPage = Math.max(1, endPage - maxVisiblePages + 1);
            }
            
            // Кнопка "Предыдущая"
            paginationHtml += `
                <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                    <a class="page-link" href="#" data-page="${currentPage - 1}">Предыдущая</a>
                </li>
            `;
            
            // Первая страница
            if (startPage > 1) {
                paginationHtml += `
                    <li class="page-item">
                        <a class="page-link" href="#" data-page="1">1</a>
                    </li>
                `;
                if (startPage > 2) {
                    paginationHtml += `
                        <li class="page-item disabled">
                            <span class="page-link">...</span>
                        </li>
                    `;
                }
            }
            
            // Основные страницы
            for (let i = startPage; i <= endPage; i++) {
                paginationHtml += `
                    <li class="page-item ${currentPage === i ? 'active' : ''}">
                        <a class="page-link" href="#" data-page="${i}">${i}</a>
                    </li>
                `;
            }
            
            // Последняя страница
            if (endPage < totalPages) {
                if (endPage < totalPages - 1) {
                    paginationHtml += `
                        <li class="page-item disabled">
                            <span class="page-link">...</span>
                        </li>
                    `;
                }
                paginationHtml += `
                    <li class="page-item">
                        <a class="page-link" href="#" data-page="${totalPages}">${totalPages}</a>
                    </li>
                `;
            }
            
            // Кнопка "Следующая"
            paginationHtml += `
                <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
                    <a class="page-link" href="#" data-page="${currentPage + 1}">Следующая</a>
                </li>
            `;
        }
        
        pagination.html(paginationHtml);
    }
    
    // Обработчики событий
    searchInput.on('input', filterOrders);
    statusFilter.on('change', filterOrders);
    dateRangePicker.on('apply.daterangepicker', filterOrders);
    dateSort.on('change', filterOrders);
    resetDates.on('click', function() {
        dateRangePicker.data('daterangepicker').setStartDate(moment().subtract(364, 'days'));
        dateRangePicker.data('daterangepicker').setEndDate(moment().endOf('day'));
        filterOrders();
    });
    
    pagination.on('click', '.page-link', function(e) {
        e.preventDefault();
        const page = $(this).data('page');
        if (page && page !== currentPage) {
            currentPage = page;
            showCurrentPage();
            updatePagination();
        }
    });
    
    // Инициализация
    filterOrders();
}); 