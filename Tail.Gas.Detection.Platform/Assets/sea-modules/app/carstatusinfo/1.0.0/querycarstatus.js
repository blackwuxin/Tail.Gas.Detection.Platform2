define(function (require, exports, module) {

    var $ = require('jquery');
    var JSON = require('json');
    var Constants = require("constants");
    var commonData = require("commondata");
    var Utils = require("utils");
    require("bootstrap");
    require("dataTables");
    require("bootstrapDialog");
    require("dataTables.bootstrap");
    var Dialog = require('dialog');



    exports.init = function () {

        var table = null;
        $("#btn-search").click(function () {
            var CarNo = $("#CarNo").val();
            var startDateTime = $("#start-datetime").val();
            var endDateTime = $("#end-datetime").val();
            if (CarNo === "") {
                Dialog.error('车牌不能为空');
                return;
            }
            if (startDateTime === "" || endDateTime === "") {
                Dialog.error('开始时间和结束时间不能为空');
                return;
            }
            if (!table) {
                table = initDataTable();
            } else {
                table.fnDraw();
            }
            
      
        });
    }
    function initDataTable() {

        return $('#pclist').dataTable({
            'processing': true,
            'bStateSave': true,
            "bFilter": false,
            "bSort": false,
            "bServerSide": true,
            "sAjaxSource": window.BASE_PATH + '/carstatusinfo/querycarstatus2',
            "fnServerData": retrieveData,
            "columnDefs": [
                {
                    "render": function (data, type, row) {
                        if (row["SystemStatus"] == 0) {
                            return "正常";
                        } else {
                            return "异常";
                        }
                    },
                    "targets": 6
                },
                 {
                     "render": function (data, type, row) {
                         return row["Data_LastChangeTime"].replace("T", " ");
                     },
                     "targets": 13
                 },
                   

            ],
            "aoColumns": [{
                "mData": "CarNo"
            }, {
                "mData": "Color"
            },

             {
                 "mData": "TemperatureBefore"
             },
             {
                 "mData": "TemperatureAfter"
             },
             {
                 "mData": "SensorNum"
             },
              {
                  "mData": "EngineSpeed"
              }, {
                  "mData": "SystemStatus"
              }, {
                  "mData": "PositionXDegree"
              }, {
                  "mData": "PositionXM"
              }, {
                  "mData": "PositionXS"
              },
             {
                 "mData": "PositionYDegree"
             },
             {
                 "mData": "PositionYM"
             },
             {
                 "mData": "PositionYS"
             }, {
                  "mData": "Data_LastChangeTime"
              }],
            "language": Constants.DataTableLanguage
        });
    }
    function convertQueryResult(result) {
        var aaData = {};
        aaData['iDisplayLength'] = 10;

        if (result == null) {
            aaData["iTotalRecords"] = 0;
            aaData["iTotalDisplayRecords"] = 0;
            aaData["aaData"] = [];
            aaData["aaData"].push({});
        } else {
            aaData["iTotalRecords"] = result["total-records"];
            aaData["iTotalDisplayRecords"] = aaData["iTotalRecords"];
            aaData["aaData"] = [];
            for (var i = 0 ; i < result.messages.length; i++) {
                //  if ("豫AC8760,豫AR0038,豫AL1098".indexOf(result.messages[i].CarNo) != -1) {
                aaData["aaData"].push(result.messages[i]);
                //  }

            }
        }

        return aaData;
    }
    function retrieveData(sSource, aoData, fnCallback) {
        var CarNo = $("#CarNo").val();
        var startDateTime = $("#start-datetime").val();
        var endDateTime = $("#end-datetime").val();

        var iDisplayStart = Utils.getValueFromAOData(aoData, "iDisplayStart");
        var iDisplayLength = Utils.getValueFromAOData(aoData, "iDisplayLength");

        $.ajax({
            type: "get",
            contentType: "application/json; charset=utf-8",
            url: sSource,
            dataType: "json",
            data: {

                CarNo: CarNo,
                startDateTime: startDateTime,
                endDateTime: endDateTime,
                istart: iDisplayStart,
                ilen: iDisplayLength
            },
            success: function (resp) {
                fnCallback(convertQueryResult(resp));
                if (resp.errors) {
                    for (var i = 0 ; i < resp.errors.length; i++) {
                        console.log(resp.errors[i]);
                    }
                }
            },
            error: function (resp) {
                //  fnCallback(convertQueryResult(null));
            }
        });
    }
});