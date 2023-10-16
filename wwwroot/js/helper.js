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
    }
};