// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
<script type="text/javascript">
    $(document).ready(function () {
        $('#show_password').hover(function show() {
            //Change the attribute to text  
            $('#txtPassword').attr('type', 'text');
            $('.icon').removeClass('fa fa-eye-slash').addClass('fa fa-eye');
        },
            function () {
                //Change the attribute back to password  
                $('#txtPassword').attr('type', 'password');
                $('.icon').removeClass('fa fa-eye').addClass('fa fa-eye-slash');
            });
    //CheckBox Show Password  
    $('#ShowPassword').click(function () {
        $('#Password').attr('type', $(this).is(':checked') ? 'text' : 'password');  
            });  
        });
</script>

$('html').on('click', function () {
    parent.$('#frame').trigger('click');
});
$(document).on('click', function () {
    $('#user-menu').removeClass('show');
});

$(document).click(function () {
    $("#layers").toggle();
});

$(document).click(function () {
    $("#layers").css("display", "");
});

$(document.body).on('click', '.dropdown-menu li', function (event) {

    var $target = $(event.currentTarget);

    $target.closest('.btn-group')
        .find('[data-bind="label"]').text($target.text())
        .end()
        .children('.dropdown-toggle').dropdown('toggle');

    return false;

});
$(window).on('click', function () {
    $('#layers').css('display', 'none');
});

$(document).on('click', function () {
    $('#layers').hide();
})