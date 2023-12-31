﻿window.helper = {
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

    showErr: function (message, refresh = false, title = "Lỗi") {
        return Swal.fire(
            title,
            message,
            'error'
        ).then((result) => {
            if (refresh)
                location.reload();
        });
    },

    showWarn: function (message, title = "Cảnh báo") {
        return Swal.fire(
            title,
            message,
            'warning'
        );
    },

    showSuc: function (message, refresh = true, title = "Thành công") {
        return Swal.fire(
            title,
            message,
            'success',
        ).then((result) => {
            if (refresh)
                window.location.reload();
        });
    },
    showAlert: function (message = "Bạn có muốn thực hiện thao tác này?", refresh = false, title = "Chú ý") {
        return Swal.fire({
            title: title,
            text: message,
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Có",
            cancelButtonText: "không"
        });
    },
    getChange: function (e) {
        let t = $(e);
        var regx = /\D+/g;
        var number = t.val().replace(regx, "");
        t.attr('data-value', number);
        t.val(number.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
    },

    copy: function (e, input) {
        input.select();
        document.execCommand("copy");
        e.classList.add("active");
        setTimeout(function () {
            e.classList.remove("active");
        }, 2500);
    }
};

const currency = [2000, 1000, 500, 200, 100, 50, 20, 10, 5, 2, 1];


window["formatVND"] = function formatVND() {
    //add attribute data-type="currency" for input to set input type currency format
    var inputMoney = document.querySelectorAll('[data-type="formatvnd"]');
    function inputNumberTest() {
        var regx = /\D+/g;
        var number = this.value.replace(regx, "");
        this.setAttribute('data-value', number);
        return (this.value = number.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
    }
    inputMoney.forEach(function (input, index) {
        input.addEventListener("input", inputNumberTest);
        if (input.value)
            helper.getChange(input)
    });
}


window["stopLoading"] = function stopLoading(isStop) {
    if (!isStop)
        $("#overlay").fadeIn(300);
    else
        $("#overlay").fadeOut(300);
}