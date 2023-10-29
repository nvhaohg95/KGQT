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

    showErr: function (message, title = "Lỗi") {
        return Swal.fire(
            title,
            message,
            'error'
        );
    },

    showSuc: function (message, refresh = true, title = "Thành công",) {
        return Swal.fire(
            title,
            message,
            'success',
        ).then((result) => {
            if (result.isConfirmed && refresh)
                window.location.reload();
        });
    },

    getChange: function (e) {
        // 48 - 57 (0-9)
        let valueRef = e;

        var str1 = valueRef.value;
        if (!str1) return;

        if (
            str1[str1.length - 1].charCodeAt() < 48 ||
            str1[str1.length - 1].charCodeAt() > 57
        ) {
            valueRef.value = str1.substring(0, str1.length - 1);
            return;
        }

        let str = valueRef.value.replace(/,/g, "");

        let value = +str;
        this.getCurrency(value);
        valueRef.value = value.toLocaleString();
        valueRef.dataset.value = str;
        console.log(str)
    },

    getCurrency: function (value) {
        var map = new Map();
        let i = 0;
        //loop unitll value 0
        while (value) {
            //if divide in non-zero add in map
            if (Math.floor(value / currency[i]) != 0) {
                map.set(currency[i], Math.floor(value / currency[i]));
                //update value using mod
                value = value % currency[i];
            }
            i++;
        }
    },

    copy: function (e,input) {
        input.select();
        document.execCommand("copy");
        e.classList.add("active");
        setTimeout(function () {
            e.classList.remove("active");
        }, 2500);
    }
};

const currency = [2000, 1000, 500, 200, 100, 50, 20, 10, 5, 2, 1];


function formatVND() {
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
    });
}
