$(function () {
    $.validator.addMethod(
        "date",
        function (value, element) {
            var bits = value.match(/([0-9]+)/gi), str;
            if (!bits)
                return this.optional(element) || false;
            str = bits[1] + '/' + bits[0] + '/' + bits[2];
            return this.optional(element) || !/Invalid|NaN/.test(new Date(str));
        },
        "Por favor ingrese una fecha valida [dd/mm/yyyy]"
    );
});