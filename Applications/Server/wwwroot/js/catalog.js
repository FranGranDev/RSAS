$.ajaxSetup({
    headers: {
        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
    }
});


$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "Catalog?handler=Products",
        success: function (data) {

            $("#catalog").html(data);
        }
    });
    $.ajax({
        type: "GET",
        url: "Catalog?handler=CartInfo",
        success: function (data) {

            $("#cart").html(data);
        }
    });
});


$("#searchForm").submit(function (e) {
    e.preventDefault();
    var $btn = $(this).find('button[type="submit"]');
    var value = $("#searchString").val();

    $btn.prop('disabled', true).addClass('btn-loading');

    $.ajax({
        type: "POST",
        url: "Catalog?handler=Search",
        data: {searchString: value},
        success: function (result) {
            $("#catalog").html(result);
        },
        complete: function () {
            $btn.prop('disabled', false).removeClass('btn-loading');
        },
        error: function (error) {
            console.log(error);
        }

    });
});

$("#clearForm").submit(function (e) {
    e.preventDefault();
    var $btn = $(this).find('button[type="submit"]');

    $btn.prop('disabled', true).addClass('btn-loading');

    $.ajax({
        type: "POST",
        url: "Catalog?handler=Clear",
        success: function (result) {
            $("#catalog").html(result);

            $.ajax({
                type: "GET",
                url: "Catalog?handler=CartInfo",
                success: function (data) {

                    $("#cart").html(data);
                }
            });
        },
        complete: function () {
            $btn.prop('disabled', false).removeClass('btn-loading');
        },
        error: function (error) {
            console.log(error);
        }

    });
});

$(document).on('click', 'table .sort-btn', function () {

    var $catalog = $('#catalog');

    var sortBy = $(this).data('sort');
    var sortOrder = $catalog.hasClass('sort-asc') ? 'desc' : 'asc';

    $.ajax({
        url: 'Catalog?handler=Sort',
        type: 'POST',
        data: {
            sortBy: sortBy,
            sortOrder: sortOrder
        },
        success: function (result) {
            $("#catalog").html(result);
            $catalog.toggleClass('sort-asc sort-desc');
        }
    });
});

$(document).on('click', '.add-cart, .remove-cart', function () {
    var productId = $(this).data("product-id")

    var $button = $(this).prop("disabled", true)
    var $input = $('input[data-product-id="' + productId + '"]');

    var isAdd = $(this).hasClass("add-cart");
    var addValue = (isAdd ? 1 : -1)

    $.ajax({
        type: "POST",
        url: "Catalog?handler=Add",
        data: {productId: productId, quantity: addValue},
        dataType: 'json',
        success: function (result) {
            $input.val(result.quantity).trigger('change')
        },
        error: function (error) {
            console.log(error)
        },
        complete: function () {
            $button.prop("disabled", false)

            $.ajax({
                type: "GET",
                url: "Catalog?handler=CartInfo",
                success: function (data) {

                    $("#cart").html(data);
                }
            });
        }
    });
});
