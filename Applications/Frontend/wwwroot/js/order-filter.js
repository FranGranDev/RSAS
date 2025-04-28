$(document).ready(function () {
    const ordersGrid = $('#ordersGrid');
    const searchInput = $('#searchInput');
    const statusFilter = $('#statusFilter');
    const dateSort = $('#dateSort');
    const pagination = $('#pagination');
    
    let currentPage = 1;
    let itemsPerPage = 6;
    let filteredOrders = Array.from(document.querySelectorAll('.order-card'));
    
    // Функция фильтрации заказов
    function filterOrders() {
        const searchTerm = searchInput.val().toLowerCase();
        const statusValue = statusFilter.val();
        
        filteredOrders = Array.from(document.querySelectorAll('.order-card')).filter(card => {
            const id = card.dataset.id;
            const status = card.dataset.status;
            const matchesSearch = id.includes(searchTerm);
            const matchesStatus = !statusValue || status === statusValue;
            return matchesSearch && matchesStatus;
        });
        
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
                return sortValue === 'asc' ? dateA - dateB : dateB - dateA;
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
            paginationHtml += `
                <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                    <a class="page-link" href="#" data-page="${currentPage - 1}">Предыдущая</a>
                </li>
            `;
            
            for (let i = 1; i <= totalPages; i++) {
                paginationHtml += `
                    <li class="page-item ${currentPage === i ? 'active' : ''}">
                        <a class="page-link" href="#" data-page="${i}">${i}</a>
                    </li>
                `;
            }
            
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
    dateSort.on('change', filterOrders);
    
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