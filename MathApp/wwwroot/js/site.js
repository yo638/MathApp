
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
    /*$(document).ready(function () {
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
*/
/*
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
})*/


function validateUserRegistration() {
    let isValid = true;

    isValid = isValid && validateUsername();
    isValid = isValid && validateName();
    isValid = isValid && validateEmail();
    isValid = isValid && validatePassword();
    isValid = isValid && validateRepeatedPassword();
    return isValid;
}
function validateUserChangePassword() {
    let isValid = true;

    isValid = isValid && validatePassword();
    isValid = isValid && validateRepeatedPassword();
    return isValid;
}
function validateUsername() {
    document.getElementById("usernameError").innerHTML = "";
    let username = document.forms["RegisterForm"]["Username"].value;
    console.log(username);
    if (username == "") {
        document.getElementById("usernameError").innerHTML = "Моля, попълнете потребителско име.";
        return false;
    }
    else if (username.lenght > 50) { //validate if the username is the correct lenght
        document.getElementById("usernameError").innerHTML = "Потребителското име трябва да съдържа между 1 и 50 символа.";
        return false;
    }
    else if (!(/^[A-Za-z0-9_.]{1,50}$/.test(username))) { //validate if the username is the correct format
        document.getElementById("usernameError").innerHTML = "Потребителското име може да съдържа само букви, цифри и символите '_' и '.'";
        return false;
    }
    return true;
}
function validateName() {
    document.getElementById("nameError").innerHTML = "";
    let name = document.forms["RegisterForm"]["Name"].value;
    console.log(name);
    if (name == "") {
        document.getElementById("nameError").innerHTML = "Моля, попълнете вашето име.";
        return false;
    }
    else if (name.lenght > 50) { //validate if the name is the correct lenght
        document.getElementById("nameError").innerHTML = "Името трябва да съдържа между 1 и 50 символа.";
        return false;
    }
    else if (!(/^[A-Za-z0-9_.]{1,50}$/.test(name))) { //validate if the username is the correct format
        document.getElementById("nameError").innerHTML = "Името може да съдържа само букви, цифри и символите '_' и '.'";
        return false;
    }
    return true;
}

function validateEmail() {
    document.getElementById("emailError").innerHTML = "";
    let email = document.forms["RegisterForm"]["Email"].value;
    console.log(email);
    if (email == "") {
        document.getElementById("emailError").innerHTML = "Моля, попълнете имейл.";
        return false;
    }
    else if (!(/^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.\-]+[.][a-zA-Z0-9.-]+$/.test(email))) { //validate if the email is the correct format
        document.getElementById("emailError").innerHTML = "Моля, попълнете валиден имейл.";
        return false;
    }
    return true;
}
function validatePassword() {
    document.getElementById("passwordError").innerHTML = "";
    let password = document.forms["RegisterForm"]["Password"].value;
    console.log(password);
    if (password == "") {
        document.getElementById("passwordError").innerHTML = "Моля, попълнете парола.";
        return false;
    }
    else if (!(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d~`!@#$%^&*()_\-+={[}\]|\\:;"'<,>.?\/]{8,50}$/.test(password))) { //validate if the password is the correct format
        document.getElementById("passwordError").innerHTML = "Паролата трябва да съдържа минимум 8 символа, поне една главна буква, поне една малка буква и поне една цифра.";
        return false;
    }
    return true;
}
function validateRepeatedPassword() {
    document.getElementById("repeatPasswordError").innerHTML = "";
    let password = document.forms["RegisterForm"]["Password"].value;
    console.log(password);
    let repeatPassword = document.forms["RegisterForm"]["RepeatPassword"].value;
    console.log(repeatPassword);
    if (password != repeatPassword) {
        document.getElementById("repeatPasswordError").innerHTML = "Паролите не съвпадат.";
        return false;
    }
    return true;
}