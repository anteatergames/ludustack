var EXPORTHELPER = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    const uri = 'data:application/vnd.ms-excel;base64,';

    const template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>';

    function init() {
        setSelectors();
        cacheObjects();

        bindAll();
    }

    function setSelectors() {
        selectors.xxx = '#xxx';
    }

    function cacheObjects() {
        objs.xxx = $(selectors.xxx);
    }

    function bindAll() {
    }

    function tableToExcel(table, name) {
        if (!table.nodeType) table = document.getElementById(table)
        var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML }
        window.location.href = uri + base64(format(template, ctx))
    }

    function base64(s) {
        return window.btoa(unescape(encodeURIComponent(s)))
    }

    function format(s, c) {
        return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; })
    }

    return {
        Init: init,
        TableToExcel: tableToExcel
    };
}());

EXPORTHELPER.Init();