

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

var helper = function () {
    var form2Object = function (form) {
        const data = JSON.parse(JSON.stringify(form));
        let obj = {};
        for (var i in data) {
            obj[data[i].name] = data[i].value
        }
        return obj;
    }
}(helper);

window["helper"] = helper;