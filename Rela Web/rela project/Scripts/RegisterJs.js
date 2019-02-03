/// <reference path="jquery-3.3.1.min.js" /

$("#password").keyup(function () {
    alert("Hello world");
    var password = $(this).val();
    var errorPassword = "";
    if (password.length < 6 && password > 17) 
        errorPassword += "Your password must be 6 to 17 characters";    
    if (/[0-9]/.test(password))
        errorPassword += "\nYour password must contain number";
    if (/[a-z]/.test(password))
        errorPassword += "\nYour password must contain letter";
    if (/[A-Z]/.test(password))
        errorPassword += "\nYour password must contain big letter";
    if (/[ !@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password))
        errorPassword += "\nYour password must contain special character";
    $("txtPasswordRequirments").text("");
    $("txtPasswordRequirments").text(errorPassword);


});