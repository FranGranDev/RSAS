// Настройка toastr
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

// Единый объект для уведомлений
const notification = {
    showSuccess: function(message) {
        toastr.success(message);
    },
    showError: function(message) {
        toastr.error(message);
    },
    showWarning: function(message) {
        toastr.warning(message);
    },
    showInfo: function(message) {
        toastr.info(message);
    }
};

// Очистка ошибок при изменении полей
$(document).ready(function() {
    $('input').on('input', function() {
        var fieldName = $(this).attr('name');
        if (fieldName) {
            $('span[data-valmsg-for="' + fieldName + '"]').text('');
        }
    });
}); 