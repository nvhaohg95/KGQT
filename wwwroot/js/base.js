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