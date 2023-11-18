// click ChangePass
function clickChangePass() {
    var data = $("#formChangePass").serialize();
    $.ajax({
        url: '/home/changePassword',
        type: "POST",
        data: data,
        success: function (respone) {
            if (!respone.isError)
            {
                helper.showSuc('Cập nhật thành công!')
                $(location).attr('href', '/auth/login');
            }
            else
                helper.showErr('Hệ thống thực thi không thành công. Vui lòng thử lại!')
        },
        error: function ()
        {
            helper.showErr('Hệ thống thực thi không thành công. Vui lòng thử lại!')
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

// reset form
function resetForm() {
    $(':input', '#myform')
        .not(':button, :submit, :reset, :hidden')
        .val('')
}

// serach from
function sort(status) {
    var ID = $('#myform').find('input[name="ID"]').val();
    var fromDate = $('#myform').find('input[name="fromDate"]').val();
    var toDate = $('#myform').find('input[name="toDate"]').val();
    var params = `?status=${status}`;
    if (ID != "") {
        params += `&ID=${ID}`;
    }
    if (fromDate != "") {
        params += `&fromDate=${fromDate}`;
    }
    if (toDate != "") {
        params += `&toDate=${toDate}`;
    }
    window.history.replaceState(null, null, params);
    location.reload()
}


