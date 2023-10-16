// click ChangePass
function clickChangePass() {
    var data = $("#formChangePass").serialize();
    $.ajax({
        url: '/home/changePassword',
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        success: function (respone) {
            if (!respone.isError) {
                $(location).attr('href', '/auth/login');
            }
        }
    });
}


// show/hide password
function showHidePass(idIcon, idInput) {
    let _input = $(idInput);
    let _icon = $(idIcon);
    if (_icon.hasClass('fa fa-eye-slash')) {
        _icon.removeClass('fa fa-eye-slash');
        _icon.addClass('fa fa-eye');
        _input.attr('type', 'text');
    }
    else {
        _icon.removeClass('fa fa-eye');
        _icon.addClass('fa fa-eye-slash');
        _input.attr('type', 'password');
    }
};

function debounce(func, within = 300, timerId = null) {
    window.callOnceTimers = window.callOnceTimers || {};
    if (timerId == null)
        timerId = func;
    var timer = window.callOnceTimers[timerId];
    clearTimeout(timer);
    timer = setTimeout(() => func(), within);
    window.callOnceTimers[timerId] = timer;
}
