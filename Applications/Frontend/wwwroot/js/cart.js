function updateQuantity(productId, quantity) {
    quantity = Math.max(0, parseInt(quantity));
    document.getElementById(`quantity-${productId}`).value = quantity;
    
    fetch('/Client/Catalog/UpdateCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({
            productId: productId,
            quantity: quantity
        })
    })
    .then(response => response.json())
    .then(data => {
        updateCartUI(data);
    })
    .catch(error => {
        console.error('Error:', error);
        toastr.error('Произошла ошибка при обновлении корзины');
    });
}

function increaseQuantity(productId) {
    const input = document.getElementById(`quantity-${productId}`);
    const newQuantity = parseInt(input.value) + 1;
    updateQuantity(productId, newQuantity);
}

function decreaseQuantity(productId) {
    const input = document.getElementById(`quantity-${productId}`);
    const newQuantity = Math.max(0, parseInt(input.value) - 1);
    updateQuantity(productId, newQuantity);
}

function clearCart() {
    fetch('/Client/Catalog/ClearCart', {
        method: 'POST',
        headers: {
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    })
    .then(response => response.json())
    .then(data => {
        updateCartUI(data);
        // Сброс количества всех товаров
        document.querySelectorAll('input[type="number"]').forEach(input => {
            input.value = 0;
        });
    })
    .catch(error => {
        console.error('Error:', error);
        toastr.error('Произошла ошибка при очистке корзины');
    });
}

function updateCartUI(cartData) {
    // Обновление количества товаров в корзине
    const cartQuantity = document.querySelector('.bi-cart3 + .badge');
    if (cartQuantity) {
        cartQuantity.textContent = cartData.totalQuantity;
    }

    // Обновление общей суммы
    const cartTotal = document.querySelector('.fw-bold');
    if (cartTotal) {
        cartTotal.textContent = new Intl.NumberFormat('ru-RU', {
            style: 'currency',
            currency: 'RUB'
        }).format(cartData.totalPrice);
    }

    // Обновление состояния кнопки "Оформить заказ"
    const orderButton = document.querySelector('a[href="/Client/Order/Create"]');
    if (orderButton) {
        orderButton.classList.toggle('disabled', cartData.totalQuantity === 0);
    }
} 