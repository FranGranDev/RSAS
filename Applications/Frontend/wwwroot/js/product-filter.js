document.addEventListener('DOMContentLoaded', function() {
    const productsGrid = document.getElementById('productsGrid');
    const searchInput = document.getElementById('searchInput');
    const categoryFilter = document.getElementById('categoryFilter');
    const priceSort = document.getElementById('priceSort');
    const pagination = document.getElementById('pagination');

    const itemsPerPage = 8;
    let currentPage = 1;
    let filteredProducts = Array.from(document.querySelectorAll('.product-card'));

    // Функция для фильтрации товаров
    function filterProducts() {
        const searchTerm = searchInput.value.toLowerCase();
        const selectedCategory = categoryFilter.value;

        filteredProducts = Array.from(document.querySelectorAll('.product-card')).filter(card => {
            const name = card.dataset.name.toLowerCase();
            const category = card.dataset.category;
            const matchesSearch = name.includes(searchTerm);
            const matchesCategory = !selectedCategory || category === selectedCategory;
            return matchesSearch && matchesCategory;
        });

        // Сортировка по цене
        if (priceSort.value) {
            filteredProducts.sort((a, b) => {
                const priceA = parseFloat(a.dataset.price);
                const priceB = parseFloat(b.dataset.price);
                return priceSort.value === 'asc' ? priceA - priceB : priceB - priceA;
            });
        }

        updatePagination();
        showCurrentPage();
    }

    // Функция для отображения текущей страницы
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

    // Функция для обновления пагинации
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

    // Обработчики событий для фильтров
    searchInput.addEventListener('input', filterProducts);
    categoryFilter.addEventListener('change', filterProducts);
    priceSort.addEventListener('change', filterProducts);

    // Инициализация
    filterProducts();
});