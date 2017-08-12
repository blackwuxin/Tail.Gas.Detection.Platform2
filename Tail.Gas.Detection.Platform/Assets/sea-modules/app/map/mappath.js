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
        initMap();
        $("#btn-search").click(function () {
            var CarNo = $("#CarNo").val();
            var startDateTime = $("#start-datetime").val();
            var endDateTime = $("#end-datetime").val();
            if (CarNo === "") {
                Dialog.error('车牌不能为空');
                return;
            }
            $.ajax({
                type: "get",
                contentType: "application/json; charset=utf-8",
                url: window.BASE_PATH + '/map/getmappath',
                dataType: "json",
                data: {
                    CarNo: CarNo,
                    startDateTime: startDateTime,
                    endDateTime: endDateTime
                },
                success: function (res) {
                    console.log(res);
                    if (res.messages && res.messages.length) {
                        var data_info = [];
                        for (var i = 0, l = res.messages.length; i < l; i++) {
                            var message = res.messages[i];

                            if ((message.PositionXDegree === 0 && message.PositionXM === 0 && message.PositionXS === 0)
                                || (message.PositionYDegree === 0 && message.PositionYM === 0 && message.PositionYS === 0)
                                || message.PositionXM > 60 || message.PositionXS > 6000
                                || message.PositionYM > 60 || message.PositionYS > 6000) {
                                continue;
                            }

                            var lng = message.PositionXDegree + message.PositionXM / 60 + message.PositionXS / 100 / 3600;
                            var lat = message.PositionYDegree + message.PositionYM / 60 + message.PositionYS / 100 / 3600;
                            var temperatureBefore = message.TemperatureBefore;
                            var temperatureAfter = message.TemperatureAfter;
                            var carno = message.CarNo;
                            var sensornum = message.SensorNum;
                            var data_lasttime = message.Data_LastChangeTime;
                            //var point = [lng, lat, '车牌 : ' + carno +
                            //    (temperatureBefore > 0 ? '<br>排气温度 : ' + temperatureBefore + '度' : '') +
                            //    (temperatureAfter > 0 ? '<br>再生温度 : ' + temperatureAfter + '度' : '') +
                            //     (sensornum > 0 ? '<br>压力 : ' + sensornum + 'Kpa' : '') +
                            //     '<br>时间 : ' + data_lasttime
                            //];
                            var point = [lng, lat, '车牌 : ' + carno +
                             '<br>排气温度 : ' + temperatureBefore + '度' +
                             '<br>再生温度 : ' + temperatureAfter + '度' +
                             '<br>压力 : ' + sensornum + 'Kpa' +
                              '<br>时间 : ' + data_lasttime
                            ];
                            data_info.push(point);
                        }
                        var pointArr = [];
                        for (var i = 0; i < data_info.length; i++) {
                            var ggPoint = new BMap.Point(data_info[i][0], data_info[i][1]);
                            pointArr.push(ggPoint);
                        }

                        var convertor = new BMap.Convertor();

                        for (var i = 0; i < pointArr.length; i = i + 10) {
                            map.clearOverlays();
                            var obj = pointArr.slice(i, i + 10);
                            convertor.translate(obj, 1, 5, function (data) {
                                if (data.status == 0) {
                                    for (var i = 0; i < data.points.length; i++) {
                                        var marker = new BMap.Marker(data.points[i]);
                                        map.addOverlay(marker);
                                       // map.setCenter(data.points[i]);
                                        var content = data_info[i][2];
                                        addClickHandler(content, marker);
                                        lastpoint = data.points[i];
                                    }
                                    map.setCenter(data.points[0])
                                }
                            });
                        }


                    }
                    else {
                        Dialog.info('无数据');
                    }
                },
                error: function (res) {
                    console.log(res);
                    Dialog.error('查询失败');
                }
            });
        });
    }
    function initMap() {
        // 百度地图API功能
        map = new BMap.Map("allmap");
        map.centerAndZoom('哈尔滨', 10);
        // 添加带有定位的导航控件
        var navigationControl = new BMap.NavigationControl({
            // 靠左上角位置
            anchor: BMAP_ANCHOR_TOP_LEFT,
            // LARGE类型
            type: BMAP_NAVIGATION_CONTROL_LARGE,
            // 启用显示定位
            enableGeolocation: false
        });
        map.addControl(navigationControl);
        map.enableScrollWheelZoom(true);
    }
    function addClickHandler(content, marker) {
        marker.addEventListener("click", function (e) {
            openInfo(content, e)
        });
    }
    function openInfo(content, e) {
        var opts = {
            width: 250,     // 信息窗口宽度
            height: 120,     // 信息窗口高度
            title: "<h4>车辆详情</h4>", // 信息窗口标题
            enableMessage: true//设置允许信息窗发送短息
        };
        var p = e.target;
        var point = new BMap.Point(p.getPosition().lng, p.getPosition().lat);
        var infoWindow = new BMap.InfoWindow(content, opts);  // 创建信息窗口对象
        map.openInfoWindow(infoWindow, point); //开启信息窗口
    }
});