// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $('[data-tooltip="tooltip"]').tooltip()
    $('[data-toggle="tooltip"]').tooltip()
})

$(document).ready(function () {
    // Control de fecha de Contrato/Create y Contrato/Edit
    $('#FechaDesde').change(function () {
        var fechaDesde = new Date($('#FechaDesde').val());
        fechaDesde.setDate(fechaDesde.getDate() + 1);
        var mesDesde = fechaDesde.getMonth() + 1;
        var anioDesde = fechaDesde.getFullYear();

        var fechaHasta = new Date($('#FechaHasta').val());
        fechaHasta.setDate(fechaHasta.getDate() + 1);
        var mesHasta = fechaHasta.getMonth() + 1;
        var anioHasta = fechaHasta.getFullYear();

        var mesMinimoHasta = ("0" + (fechaDesde.getMonth() + 2)).slice(-2);
        var anioMinimoHasta = anioDesde;

        if (mesMinimoHasta == 13) {
            mesMinimoHasta = "01";
            anioMinimoHasta++;
        }

        $('#FechaHasta').attr({
            "min": [anioMinimoHasta, mesMinimoHasta].join('-')
        });

        if (anioDesde > anioHasta || (anioDesde == anioHasta && mesDesde >= mesHasta)) {
            $('#FechaHasta').val("");
            //$('#FechaHasta').val([anioMinimoHasta, mesMinimoHasta].join('-'));
        }
    });
});