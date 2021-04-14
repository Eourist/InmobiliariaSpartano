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
        // Eprolijar
        var date = new Date($('#FechaDesde').val());
        date.setDate(date.getDate() + 1);
        var m = date.getMonth() + 1;
        var month = ("0" + (date.getMonth() + 2)).slice(-2); // 4 -> 05
        var year = date.getFullYear();

        var dateHasta = new Date($('#FechaHasta').val());
        dateHasta.setDate(dateHasta.getDate() + 1);
        var mh = dateHasta.getMonth() + 1;
        var yearHasta = dateHasta.getFullYear();

        if (year > yearHasta || (year == yearHasta && m >= mh)) {
            if (month == 13) {
                month = "01"; year = year + 1;
            }
            $('#FechaHasta').val([year, month].join('-'));
            $('#FechaHasta').attr({
                "min": [year, month].join('-')
            });
        }
    });
});