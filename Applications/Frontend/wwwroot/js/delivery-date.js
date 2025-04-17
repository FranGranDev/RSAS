document.addEventListener('DOMContentLoaded', function() {
    const deliveryDateInput = document.querySelector('input[name="DeliveryDate"]');
    if (deliveryDateInput) {
        // Устанавливаем начальное значение в формате дд.мм.гггг чч:00
        formatDeliveryDate(deliveryDateInput);

        // Обработчик изменения даты
        deliveryDateInput.addEventListener('change', function() {
            formatDeliveryDate(this);
        });
    }
});

function formatDeliveryDate(input) {
    if (input.value) {
        const date = new Date(input.value);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();
        const hours = String(date.getHours()).padStart(2, '0');
        
        // Устанавливаем значение в формате дд.мм.гггг чч:00
        input.value = `${day}.${month}.${year} ${hours}:00`;
    }
} 