$(document).ready(function () {

    $('#btnAutenticar').click(function () {
        $("#div-loader").show();
        var object = new Object();
        object.NombreDeUsuario = $("#NombreDeUsuario").val();
        object.Password = $("#Password").val();
        object.TerminalId = $("#TerminalId option:selected").val();
        var data = JSON.stringify(object);

        autenticar(data);

        //var respuesta = getDuplicates(object.NombreDeUsuario, object.TerminalId);

        //if (respuesta.isDuplicate) {
        //    $("#div-loader").hide();
        //    Lobibox.confirm({
        //        title: 'Atención',
        //        msg: "La terminal se encuntra en uso por el usuario: " + respuesta.usuario + " ¿Desea continuar con esta terminal?",
        //        callback: function ($this, type, ev) {
        //            if (type === 'yes') {
        //                autenticar(data);
        //            }
        //            else {
        //                return;
        //            }
        //        }

        //    });

        //    return;
        //}





    });

    function autenticar(data) {
        $.ajax({
            type: 'POST',
            url: 'Login/Autenticar',
            data: data,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (dataResponse) {
                $("#div-loader").hide();
                if (dataResponse.response) {
                    $("#div-loader").hide();
                    window.location.href = dataResponse.href;
                } else {
                    $("#div-loader").hide();
                    Lobibox.notify('error', {
                        title: 'Error',
                        msg: 'Ah ocurrido un error, intente nuevamente.'
                    });
                }
            },
            error: function () {
                $("#div-loader").hide();
                Lobibox.notify('error', {
                    title: 'Error',
                    msg: 'Ah ocurrido un error, intente nuevamente.'
                });
            }
        });
    }

    function getDuplicates(user, terminalId) {
        var respuesta = {
            isDuplicate: false,
            usuario: null
        };

        var data = {
            usuario: user,
            terminal: terminalId
        };

        $.ajax({
            type: 'GET',
            async: false,
            url: 'Terminals/GetTerminalAsigned',
            data: data,
            cache: false,
            success: function (result) {
                if (result.responseCode > 0) {
                    respuesta.isDuplicate = true;
                    respuesta.usuario = result.usuario;
                }
                else {
                    respuesta.isDuplicate = false;
                }
            }
        });

        return respuesta;
    }

});