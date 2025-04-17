document.addEventListener('DOMContentLoaded', function() {
    // Функция для обновления количества товаров в мини-корзине
    function updateMiniCart(quantity) {
        const miniCartBadge = document.getElementById('cart-mini-total-quantity');
        if (miniCartBadge) {
            if (quantity > 0) {
                miniCartBadge.textContent = quantity;
                miniCartBadge.style.display = 'block';
            } else {
                miniCartBadge.style.display = 'none';
            }
        }
    }

    // Подписываемся на событие обновления корзины
    document.addEventListener('cartUpdated', function(e) {
        updateMiniCart(e.detail.totalQuantity);
    });
}); 