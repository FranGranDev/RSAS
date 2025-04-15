document.addEventListener('DOMContentLoaded', function() {
    const productsGrid = document.getElementById('productsGrid');
    const searchInput = document.getElementById('searchInput');
    const searchButton = document.getElementById('searchButton');
    const sortBy = document.getElementById('sortBy');
    const sortDirection = document.getElementById('sortDirection');
    const categoryFilter = document.getElementById('categoryFilter');
    const pagination = document.getElementById('pagination');

    const itemsPerPage = 8;
    let currentPage = 1;
    let filteredProducts = Array.from(document.querySelectorAll('.product-card'));
    let isAscending = true;

    function updateSortIcon() {
        const icon = sortDirection.querySelector('i');
        icon.className = isAscending ? 'bi bi-sort-down' : 'bi bi-sort-up';
    }

    function filterAndSortProducts() {
        const searchTerm = searchInput.value.toLowerCase();
        const sortField = sortBy.value;
        const selectedCategory = categoryFilter.value;
        
        // Фильтрация
        filteredProducts = Array.from(document.querySelectorAll('.product-card')).filter(card => {
            const name = card.dataset.name.toLowerCase();
            const category = card.dataset.category.toLowerCase();
            const matchesSearch = name.includes(searchTerm) || category.includes(searchTerm);
            const matchesCategory = !selectedCategory || card.dataset.category === selectedCategory;
            return matchesSearch && matchesCategory;
        });
        
        // Сортировка
        filteredProducts.sort((a, b) => {
            let valueA, valueB;
            
            switch (sortField) {
                case 'name':
                    valueA = a.dataset.name;
                    valueB = b.dataset.name;
                    break;
                case 'category':
                    valueA = a.dataset.category;
                    valueB = b.dataset.category;
                    break;
                case 'quantity':
                    valueA = parseInt(a.dataset.quantity);
                    valueB = parseInt(b.dataset.quantity);
                    break;
                default:
                    return 0;
            }
            
            if (isAscending) {
                return valueA > valueB ? 1 : -1;
            } else {
                return valueA < valueB ? 1 : -1;
            }
        });

        currentPage = 1;
        updatePagination();
        showCurrentPage();
    }

    function showCurrentPage() {
        const start = (currentPage - 1) * itemsPerPage;
        const end = start + itemsPerPage;

        // Скрываем все карточки
        document.querySelectorAll('.product-card').forEach(card => {
            card.style.display = 'none';
        });

        // Показываем только карточки текущей страницы
        filteredProducts.slice(start, end).forEach(card => {
            card.style.display = 'block';
        });
    }

    function updatePagination() {
        const totalPages = Math.ceil(filteredProducts.length / itemsPerPage);
        let paginationHTML = '';

        // Кнопка "Назад"
        paginationHTML += `
            <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link" href="#" data-page="${currentPage - 1}">Назад</a>
            </li>
        `;

        // Номера страниц
        for (let i = 1; i <= totalPages; i++) {
            paginationHTML += `
                <li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>
            `;
        }

        // Кнопка "Вперед"
        paginationHTML += `
            <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link" href="#" data-page="${currentPage + 1}">Вперед</a>
            </li>
        `;

        pagination.innerHTML = paginationHTML;

        // Добавляем обработчики событий для кнопок пагинации
        pagination.querySelectorAll('.page-link').forEach(link => {
            link.addEventListener('click', function(e) {
                e.preventDefault();
                const page = parseInt(this.dataset.page);
                if (page >= 1 && page <= totalPages) {
                    currentPage = page;
                    showCurrentPage();
                    updatePagination();
                }
            });
        });
    }

    // Обработчики событий
    searchInput.addEventListener('input', filterAndSortProducts);
    searchButton.addEventListener('click', filterAndSortProducts);
    sortBy.addEventListener('change', filterAndSortProducts);
    sortDirection.addEventListener('click', function() {
        isAscending = !isAscending;
        updateSortIcon();
        filterAndSortProducts();
    });
    categoryFilter.addEventListener('change', filterAndSortProducts);

    // Инициализация
    updateSortIcon();
    filterAndSortProducts();
}); 