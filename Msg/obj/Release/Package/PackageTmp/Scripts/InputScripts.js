$(document).ready(function () {

    var placeholderVal;
    $(".formInput").focus(function () {
        placeholderVal = $(this).attr("placeholder");
        $(this).attr("placeholder", '');
    });
    $(".formInput").focusout(function () {
        $(this).attr("placeholder", placeholderVal);
    });

   

});