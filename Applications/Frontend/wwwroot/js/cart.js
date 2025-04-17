function updateQuantity(productId, quantity) {
    $.ajax({
        type: 'POST',
        url: '/Client/Catalog?handler=UpdateCart',
        data: JSON.stringify({ productId: productId, quantity: quantity }),
        contentType: 'application/json',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (result) {
            // Обновляем количество в карточке товара
            $('#quantity-' + productId).val(quantity);
            $('#quantity-preview-' + productId).val(quantity);
            
            // Обновляем информацию о корзине
            $('#cart-total-quantity').text(result.totalQuantity);
            $('#cart-total-price').text(result.totalPrice.toFixed(2) + ' р.');
            
            // Обновляем состояние кнопки "Оформить заказ"
            $('#create-order-btn').toggleClass('disabled', result.totalQuantity === 0);

            // Генерируем событие обновления корзины
            const event = new CustomEvent('cartUpdated', {
                detail: {
                    totalQuantity: result.totalQuantity,
                    totalPrice: result.totalPrice
                }
            });
            document.dispatchEvent(event);
        },
        error: function (error) {
            console.error('Ошибка при обновлении корзины:', error);
            notification.showError('Произошла ошибка при обновлении корзины');
        }
    });
}

function increaseQuantity(productId) {
    var input = $('#quantity-' + productId);
    var previewInput = $('#quantity-preview-' + productId);
    var newValue = parseInt(input.val()) + 1;
    input.val(newValue).trigger('change');
    previewInput.val(newValue);
}

function decreaseQuantity(productId) {
    var input = $('#quantity-' + productId);
    var previewInput = $('#quantity-preview-' + productId);
    var newValue = Math.max(0, parseInt(input.val()) - 1);
    input.val(newValue).trigger('change');
    previewInput.val(newValue);
}

function clearCart() {
    $.ajax({
        type: 'POST',
        url: '/Client/Catalog?handler=ClearCart',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (result) {
            // Сброс количества всех товаров
            $('input[type="number"]').val(0);
            
            // Обновляем информацию о корзине
            $('#cart-total-quantity').text(result.totalQuantity);
            $('#cart-total-price').text(result.totalPrice.toFixed(2) + ' р.');
            
            // Обновляем состояние кнопки "Оформить заказ"
            $('#create-order-btn').toggleClass('disabled', result.totalQuantity === 0);

            // Генерируем событие обновления корзины
            const event = new CustomEvent('cartUpdated', {
                detail: {
                    totalQuantity: result.totalQuantity,
                    totalPrice: result.totalPrice
                }
            });
            document.dispatchEvent(event);
        },
        error: function (error) {
            console.error('Ошибка при очистке корзины:', error);
            notification.showError('Произошла ошибка при очистке корзины');
        }
    });
} 