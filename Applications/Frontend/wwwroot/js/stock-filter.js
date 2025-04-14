document.addEventListener('DOMContentLoaded', function() {
    const stocksGrid = document.getElementById('stocksGrid');
    const searchInput = document.getElementById('searchInput');
    const searchButton = document.getElementById('searchButton');

    function filterStocks() {
        const searchTerm = searchInput.value.toLowerCase();
        const stockCards = document.querySelectorAll('.stock-card');

        stockCards.forEach(card => {
            const stockName = card.dataset.name.toLowerCase();
            if (stockName.includes(searchTerm)) {
                card.style.display = 'block';
            } else {
                card.style.display = 'none';
            }
        });
    }

    searchInput.addEventListener('input', filterStocks);
    searchButton.addEventListener('click', filterStocks);
}); 