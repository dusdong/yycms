//为Array添加Contains方法  
Array.prototype.contains = function (obj) {
    var i = this.length;
    while (i--) {
        if (this[i] === obj) {
            return true;
        }
    }
    return false;
}

var APP = new Object();

/*导出数据表格*/
APP.export = function (ele)
{
    if ($("#ExportMainTable").length < 1)
    {
        $("body").append('<table id="ExportMainTable" style="display:none;" class="table table-bordered table-striped"><thead></thead><tbody></tbody></table>');
    }
    else
    {
        $("#ExportMainTable thead,#ExportMainTable tbody").empty();
    }
    var thead_new = "";
    var theads = $("#MainTable thead tr th").length - 1;
    for (var i = 0; i < theads; i++)
    {
        thead_new += '<th>' + $("#MainTable thead tr th:eq(" + i + ")").text() + '</th>';
    }
    $("#ExportMainTable thead").html("<tr>" + thead_new + "</tr>");
    var tbody_new = "";
    var tbodys = $("#MainTable tbody tr").length;
    for (var i = 0; i < tbodys; i++)
    {
        tbody_new += '<tr>';
        for (var j = 0; j < theads; j++)
        {
            tbody_new += '<td>' + $("#MainTable tbody tr:eq(" + i + ") td:eq(" + j + ")").html() + '</td>';
        }
        tbody_new += '</tr>';
    }
    $("#ExportMainTable tbody").html(tbody_new)
    return ExcellentExport.excel(ele, 'ExportMainTable');
}

/*获取url中"?"符后的字串*/
APP.Request = new Object();

APP.Title = "运营中心";

APP.Range = function (count)
{
    var result = new Array();

    for (var i = 0; i < count; i++)
    {
        result.push(i);
    }
    return result;
}

/*定义指令*/
APP._ViewTag = [
    { name: "tablePager", restrict: "E", temp: "/Directives/table_pager.html", scope: false, controller: "GlobalCtrl" },
    { name: "tablePagerTop", restrict: "E", temp: "/Directives/table_pager_top.html", scope: false, controller: "GlobalCtrl" },
    { name: "tableOperation", restrict: "E", temp: "/Directives/table_operation.html", scope: false },
    { name: "filterBtn", restrict: "E", temp: "/Directives/filter_button.html", scope: false },
    { name: "loading", restrict: "E", temp: "/Directives/page_loading.html", scope: false },
    { name: "dialogDelete", restrict: "E", temp: "/Directives/dialog_delete.html", scope: false },
    { name: "submitBtn", restrict: "E", temp: "/Directives/page_submit.html", scope: false, controller: "GlobalCtrl" },
];

APP._Filter = [
    {
        name: "substring", fn: function (txt, start, end) {
            if (end) return txt.substring(start, end);
            return txt.substring(start);
        },
        name: 'indexof', fn: function (items, key, dsasda) {
            debugger

            if (item.DeptIDs.split(',').contains(item)) {
                return item;
            }
            return null;
        }
    }
];

APP.Platforms = [{ "ID": 1, "Name": "微信", "Desc": "" }];

APP._Init = function ()
{
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "positionClass": "toast-bottom-right",
        "showDuration": "300",
        "hideDuration": "100000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    //$(document).skylo({ state: 'warning' });
    var app = angular.module('app', ["ngRoute", "ngResource", "ngSanitize", "ngCookies"]);
    $.each(APP._ViewTag, function (a, b)
    {
        app.directive(b.name, function ()
        {
            return { restrict: b.restrict, templateUrl: b.temp, scope: b.scope == false ? false : true, replace: true, controller: b.controller == undefined ? undefined : b.controller };
        });
    });
    $.each(APP._Filter, function (a, b) { app.filter(b.name, function () { return b.fn; }); });
}

APP._Init();