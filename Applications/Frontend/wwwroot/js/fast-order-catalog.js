$(document).ready(function () {
    // Обработчик изменения количества товара
    $('.quantity-input').on('change', function () {
        var $input = $(this);
        var productId = $input.data('product-id');
        var quantity = parseInt($input.val());
        var maxQuantity = parseInt($input.data('max'));

        // Проверяем, что количество не превышает максимальное
        if (quantity > maxQuantity) {
            quantity = maxQuantity;
            $input.val(quantity);
        }

        // Проверяем, что количество не отрицательное
        if (quantity < 0) {
            quantity = 0;
            $input.val(quantity);
        }

        updateCart(productId, quantity);
    });

    // Обработчик кнопки добавления товара
    $('.add-cart').on('click', function () {
        var $button = $(this);
        var productId = $button.data('product-id');
        var $input = $('input[data-product-id="' + productId + '"]');
        var currentQuantity = parseInt($input.val());
        var maxQuantity = parseInt($input.data('max'));

        if (currentQuantity < maxQuantity) {
            var newQuantity = currentQuantity + 1;
            $input.val(newQuantity);
            updateCart(productId, newQuantity);
        }
    });

    // Обработчик кнопки удаления товара
    $('.remove-cart').on('click', function () {
        var $button = $(this);
        var productId = $button.data('product-id');
        var $input = $('input[data-product-id="' + productId + '"]');
        var currentQuantity = parseInt($input.val());

        if (currentQuantity > 0) {
            var newQuantity = currentQuantity - 1;
            $input.val(newQuantity);
            updateCart(productId, newQuantity);
        }
    });

    // Функция обновления корзины
    function updateCart(productId, quantity) {
        $.ajax({
            type: "POST",
            url: "?handler=UpdateCart",
            data: JSON.stringify({ productId: productId, quantity: quantity }),
            contentType: "application/json",
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (result) {
                updateCartInfo(result.totalQuantity, result.totalPrice);
                updateButtons(productId, quantity);
            },
            error: function (error) {
                console.error('Ошибка при обновлении корзины:', error);
            }
        });
    }

    // Функция обновления информации о корзине
    function updateCartInfo(totalQuantity, totalPrice) {
        $('#cart-total-quantity').text(totalQuantity);
        $('#cart-total-price').text(totalPrice.toFixed(2) + ' ' + $('#currency-symbol').val());
        
        // Обновляем состояние кнопки оформления заказа
        var $createOrderBtn = $('#create-order-btn');
        if (totalQuantity > 0) {
            $createOrderBtn.css({
                'pointer-events': 'auto',
                'opacity': '1'
            });
        } else {
            $createOrderBtn.css({
                'pointer-events': 'none',
                'opacity': '0.65'
            });
        }
    }

    // Функция обновления состояния кнопок
    function updateButtons(productId, quantity) {
        var $input = $('input[data-product-id="' + productId + '"]');
        var maxQuantity = parseInt($input.data('max'));
        var $addButton = $('.add-cart[data-product-id="' + productId + '"]');
        var $removeButton = $('.remove-cart[data-product-id="' + productId + '"]');

        $addButton.prop('disabled', quantity >= maxQuantity);
        $removeButton.prop('disabled', quantity <= 0);
    }

    // Функция очистки корзины
    window.clearCart = function () {
        $.ajax({
            type: "POST",
            url: "?handler=ClearCart",
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (result) {
                updateCartInfo(result.totalQuantity, result.totalPrice);
                // Сбрасываем все поля ввода количества
                $('.quantity-input').val(0).each(function () {
                    var productId = $(this).data('product-id');
                    updateButtons(productId, 0);
                });
            },
            error: function (error) {
                console.error('Ошибка при очистке корзины:', error);
            }
        });
    };
}); 