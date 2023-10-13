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


function selectedTime() {
    var fromDate = "";
    var toDate = "";
    var sOption = document.getElementById("dropdowTime").value
    let date = new Date(); // get current date
    switch (sOption) {
        case "d":
            fromDate = date.toISOString().split("T")[0];
            toDate = date.toISOString().split("T")[0];
            break;
        case "w":
            let first = date.getDate() - date.getDay(), last = first + 6;
            fromDate = new Date(date.setDate(first)).toISOString().split("T")[0];
            toDate = new Date(date.setDate(last)).toISOString().split("T")[0];
            break;
        case "m":
            fromDate = new Date(date.getFullYear(), date.getMonth(), 0).toISOString().split("T")[0];
            toDate = new Date(date.getFullYear(), date.getMonth() + 1, 1).toISOString().split("T")[0];
            break;
        case "y":
            fromDate = new Date(date.getFullYear(), 0, 0).toISOString().split("T")[0];
            toDate = new Date(date.getFullYear(), 11, 31).toISOString().split("T")[0];
            break;
    }
    $("#inputFromTime").val(fromDate);
    $("#inputToTime").val(toDate);
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
window.helper = {



    form2Object: function (form) {
        const data = JSON.parse(JSON.stringify(form));
        let obj = {};
        for (var i in data) {
            let value = data[i].value;
            if (value === 'on')
                value = true;
            if (value === 'off')
                value = false;
            obj[data[i].name] = value;
        }
        return obj;
    },

    showErr = function (message, title = "Lỗi") {
        return Swal.fire(
            title,
            message,
            'error'
        );
    },

    showSuc = function (message, refresh = true, title = "Thành công",) {
        return Swal.fire(
            title,
            message,
            'success',
        ).then((result) => {
            if (result.isConfirmed && refresh)
                window.location.reload();
        });
    }
};