var helper = window["helper"] = {
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

    showSuc: function (message, refresh = true, title = "Thành công",url="") {
        return Swal.fire(
            title,
            message,
            'success',
        ).then((result) => {
            if (url)
                location.replace(url);
            else if (refresh)
                window.location.reload();
        });
    },
    showAlert: function (message = "Quý khách có muốn thực hiện thao tác này?", refresh = false, title = "Chú ý") {
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

window["copyClipboard"] = function copyClipboard(text) {
    var textArea = document.createElement("textarea");
    //
    // *** This styling is an extra step which is likely not required. ***
    //
    // Why is it here? To ensure:
    // 1. the element is able to have focus and selection.
    // 2. if the element was to flash render it has minimal visual impact.
    // 3. less flakyness with selection and copying which **might** occur if
    //    the textarea element is not visible.
    //
    // The likelihood is the element won't even render, not even a
    // flash, so some of these are just precautions. However in
    // Internet Explorer the element is visible whilst the popup
    // box asking the user for permission for the web page to
    // copy to the clipboard.
    //

    // Place in the top-left corner of screen regardless of scroll position.
    textArea.style.position = 'fixed';
    textArea.style.top = 0;
    textArea.style.left = 0;

    // Ensure it has a small width and height. Setting to 1px / 1em
    // doesn't work as this gives a negative w/h on some browsers.
    textArea.style.width = '2em';
    textArea.style.height = '2em';

    // We don't need padding, reducing the size if it does flash render.
    textArea.style.padding = 0;

    // Clean up any borders.
    textArea.style.border = 'none';
    textArea.style.outline = 'none';
    textArea.style.boxShadow = 'none';

    // Avoid flash of the white box if rendered for any reason.
    textArea.style.background = 'transparent';


    textArea.value = text;

    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();

    try {
        var successful = document.execCommand('copy');
        var msg = successful ? 'successful' : 'unsuccessful';
        console.log('Copying text command was ' + msg);
    } catch (err) {
        console.log('Oops, unable to copy');
    }
    $('#copied-success').removeClass('d-none');
    $('#copied-success').css("opacity", 1)
    setTimeout(function () {
        $('#copied-success').css("opacity", 0)
        $('#copied-success').removeClass("d-none");
    }, 500);
    document.body.removeChild(textArea);
}


