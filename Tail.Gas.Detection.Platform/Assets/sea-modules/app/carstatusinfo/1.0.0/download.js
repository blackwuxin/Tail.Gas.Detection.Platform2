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
            $.ajax({
                type: "get",
                contentType: "application/json; charset=utf-8",
                url: window.BASE_PATH + '/carstatusinfo/queryfilename',
                dataType: "json",
                data: {
                    CarNo: CarNo,
                    startDateTime: startDateTime,
                    endDateTime: endDateTime
                },
                success: function (res) {
                    console.log(res);
                    if (res.filepath) {
                        window.location.href = res.filepath;
                        Dialog.info('下载完成');
                    } else {
                        Dialog.warn('没有' + CarNo + '对应数据');
                    }
                },
                error: function (res) {
                    console.log(res);
                }
            });
        });
    }

});