
$(document).ready(function () {

    $('.Registration').click(function () {
          $.ajax({
             url: "Register",
             type: "Get",
             success: DisplayTheResultingView
          });
        location.href = 'Register';
    });

    $('.ResetPassword').click(function () {
        $.ajax({
            url: "PasswordReset",
            type: "Get",
            success: DisplayTheResultingView,
        });
        location.href = 'PasswordReset';
    });

});





function DisplayTheResultingView(View) {

    $('html').empty();
    $('html').append(View);
    var logo = $('.logo');
    $('.logo').remove();
    $('.nav-bar').children('a').first().append(logo);
   
}