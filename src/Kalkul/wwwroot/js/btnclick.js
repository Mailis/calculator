$(document).ready(function () {
    //send operator button click event to input
    //parent(op_buttons container) delegates click event to its buttons
    $(operationButtonsContainerID_selector).on('click', operationButtonsClass_selector, function () {
        var op = $(this).val();
        var expression = $(input_selector).val();
        $(input_selector).val(expression + op);
    });
    

    //send number button click event to input
    $(".btnNum").click(function () {
        var op = $(this).val();
        var expression = $(input_selector).val();
        $(input_selector).val(expression + op);
    });
    

    //empty input
    $(".btnC").click(function () {
        $(input_selector).val("");
        $(output_selector).val("");
    });


    //AJAX
    //calculate
    $("#calc_btn").click(function () {
        var calkInput = $(input_selector).val();
     
        $.get("/api/Calculation/", { input_string: calkInput })
            .done(function (data) {
                $(output_selector).val(data);
            })
            .fail(function (error) {
                $(output_selector).val(error.responseText);
                console.log("error", error);
            });
    });

});


