

const VISTA_BUSQUEDA = {

    busquedaFecha: () => { // Funcion que maneja la aparicion de inputs dependiendo de lo que selecciona el usuario  

        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroVenta").val("")

        $(".busqueda-fecha").show()
        $(".busqueda-venta").hide()

    }, busquedaVenta: () => {

        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroVenta").val("")

        $(".busqueda-fecha").hide()
        $(".busqueda-venta").show()
    }
}

$(document).ready(function () {

    VISTA_BUSQUEDA["busquedaFecha"]()

    $.datepicker.setDefaults($.datepicker.regional["es"])

    $("#txtFechaInicio").datepicker({ dateFormat: "dd/mm/yy" }) //Carga la aparicion de calendario
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })
      
})

$("#cboBuscarPor").change(function () { // Maneja que inputs se mostrara dependiendo del select en #cboBuscarPor

    if ($("#cboBuscarPor").val() == "fecha") {
        VISTA_BUSQUEDA["busquedaFecha"]()
    } else {
        VISTA_BUSQUEDA["busquedaVenta"]()
    }
})

$("#btnBuscar").click(function () { //Funcion que sirve para traer data dependiendo del filtro y datos proporcionados

    if ($("#cboBuscarPor").val() == "fecha") { //Si el usuario no ha colocado el rango de fecha se le pide que lo haga

        if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
            toastr.warning("", "Debe ingresar fecha inicio y fin")
            return;
        }

    } else { // En caso de buscar por numero de venta y no haber escrito nada se le informa

        if ($("#txtNumeroVenta").val().trim() == "") {
            toastr.warning("", "Debe ingresar el numero de venta")
            return;
        }
    }

    let numeroVenta = $("#txtNumeroVenta").val() //Capturo los datos de los inputs
    let fechaInicio = $("#txtFechaInicio").val()
    let fechaFin = $("#txtFechaFin").val()

    $(".card-body").find("div.row").LoadingOverlay("show");

    fetch(`/Venta/Historial?numeroVenta=${numeroVenta}&fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`) // Llamo al controlador y le paso los parametros
        .then(response => {
            $(".card-body").find("div.row").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            $("#tbventa tbody").html(""); // Limpio la tabla antes de mostrar la data

            if (responseJson.length > 0) {
                responseJson.forEach((venta) => { // Renderizo los datos en cada columna, muestro un boton de detalle para cada fila
                    $("#tbventa tbody").append(
                        $("<tr>").append(
                            $("<td>").text(venta.fechaRegistro),
                            $("<td>").text(venta.numeroVenta),
                            $("<td>").text(venta.tipoDocumentoVenta),
                            $("<td>").text(venta.documentoCliente),
                            $("<td>").text(venta.nombreCliente),
                            $("<td>").text(venta.total),
                            $("<td>").append(
                                $("<button>").addClass("btn btn-info btn-sm").append(
                                    $("<i>").addClass("fas fa-eye")
                                ).data("venta", venta)
                            )
                        )
                    )
                })
            }
        })


})


$("#tbventa tbody").on("click", "button.btn.btn-info", function () { // Funcion que muestra el modal de detalle de venta
    
    const data = $(this).data("venta") // obtengo la data que almacena el boton de informacion 

    $("#txtFechaRegistro").val(data.fechaRegistro) // asigno cada dato a su campo respectivo
    $("#txtNumVenta").val(data.numeroVenta)
    $("#txtUsuarioRegistro").val(data.usuario)
    $("#txtTipoDocumento").val(data.tipoDocumentoVenta)
    $("#txtDocumentoCliente").val(data.documentoCliente)
    $("#txtNombreCliente").val(data.nombreCliente)
    $("#txtSubTotal").val(data.subTotal)
    $("#txtIGV").val(data.impuestoTotal)
    $("#txtTotal").val("$"+data.total)
    $("#tbProductos tbody").html("");
    data.detalleVenta.forEach((item) => { // en el caso de la tabla renderizo la data con foreach
        $("#tbProductos tbody").append(
            $("<tr>").append(
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total)
            )
        )
    })

    $("#linkImprimir").attr("href", `/Venta/MostrarPDFVenta?numeroVenta=${data.numeroVenta}`) // Llamo al metodo generarPDF para el escenario en el que el usuario presione imprimir

    $("#modalData").modal("show"); // Muestra el Modal
    
}) 




    