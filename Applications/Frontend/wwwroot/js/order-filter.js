$(document).ready(function () {
    const ordersGrid = $('#ordersGrid');
    const searchInput = $('#searchInput');
    const statusFilter = $('#statusFilter');
    const dateFrom = $('#dateFrom');
    const dateTo = $('#dateTo');
    const dateSort = $('#dateSort');
    const pagination = $('#pagination');
    const resetDates = $('#resetDates');
    
    let currentPage = 1;
    let itemsPerPage = 6;
    let filteredOrders = Array.from(document.querySelectorAll('.order-card'));
    
    // Функция установки стандартных дат
    function setDefaultDates() {
        const today = new Date();
        const yearAgo = new Date();
        yearAgo.setFullYear(today.getFullYear() - 1);
        
        dateFrom.val(yearAgo.toISOString().split('T')[0]);
        dateTo.val(today.toISOString().split('T')[0]);
    }
    
    // Функция фильтрации заказов
    function filterOrders() {
        const searchTerm = searchInput.val().toLowerCase();
        const statusValue = statusFilter.val();
        const dateFromValue = dateFrom.val();
        const dateToValue = dateTo.val();
        
        filteredOrders = Array.from(document.querySelectorAll('.order-card')).filter(card => {
            const id = card.dataset.id;
            const status = card.dataset.status;
            const orderDate = new Date(card.dataset.date);
            
            const matchesSearch = id.includes(searchTerm);
            const matchesStatus = !statusValue || status === statusValue.toLowerCase();
            const matchesDate = (!dateFromValue || orderDate >= new Date(dateFromValue)) &&
                              (!dateToValue || orderDate <= new Date(dateToValue + 'T23:59:59'));
            
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
    dateFrom.on('change', filterOrders);
    dateTo.on('change', filterOrders);
    dateSort.on('change', filterOrders);
    resetDates.on('click', function() {
        setDefaultDates();
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
    setDefaultDates();
    filterOrders();
}); 