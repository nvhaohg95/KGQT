// click ChangePass
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
    location.reload();
}

function nextPage(crrPage, pageIndex, numberPage) {
    if (pageIndex < 1 || pageIndex > numberPage) return;
    if (location.search)
    {
        if (location.search.indexOf("page=") != -1)
            location.href = location.href.replace(`page=${crrPage}`, `page=${pageIndex}`);
        else
            location.href += `&page=${pageIndex}`;
    }
    else location.href += `?page=${pageIndex}`;
}

