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


function resetForm() {
    $(':input', '#myform')
        .not(':button, :submit, :reset, :hidden')
        .val('')
}
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

function getPage(type = "Next") {
    var urlParams = new URLSearchParams(window.location.search);
    var crrPage = parseInt(urlParams.get("page"));
    if (!crrPage)
        crrPage = 1;
    var pageNum = crrPage;
    var crrUrl = location.href;
    var newUrl = crrUrl;
    if (!crrUrl.includes("page="))
        crrUrl += `&page=${crrPage}`;
    if (type == "Next")
        pageNum = crrPage + 1;
    else
        pageNum = crrPage - 1;
    if (pageNum <= 0)
        return;
    newUrl = crrUrl.replace(`page=${crrPage}`, `page=${pageNum}`);
    location.href = newUrl;
}