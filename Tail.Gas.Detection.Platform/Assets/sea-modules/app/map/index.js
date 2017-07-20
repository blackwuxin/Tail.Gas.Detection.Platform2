define(function (require, exports, module) {

    var $ = require('jquery');
    //var BMap = require('BMap');
    var JSON = require('json');
    var belong = '哈尔滨';
    var query = function () {
        $.ajax({
            type: "get",
            contentType: "application/json; charset=utf-8",
            url: window.BASE_PATH + '/map/query',
            dataType: "json",
            data: {
                Belong: belong
            },
            success: function (res) {
                console.log(res);
                if (res.messages) {
                    map.clearOverlays()
                    var data_info = [];
                    for (var i = 0, l = res.messages.length; i < l; i++) {
                        var message = res.messages[i];
                        if ((message.PositionXDegree === 0 && message.PositionXM === 0 && message.PositionXS === 0)
                            || (message.PositionYDegree === 0 && message.PositionYM === 0 && message.PositionYS === 0))
                        {
                            continue;
                        }
                        //if (message.PositionXS > 3600) {
                        //    message.PositionXS = 0;
                        //}
                        //if (message.PositionYS > 3600) {
                        //    message.PositionYS = 0;
                        //}
                        var lng = message.PositionXDegree + message.PositionXM / 60 + message.PositionXS / 3600;
                        var lat = message.PositionYDegree + message.PositionYM / 60 + message.PositionYS / 3600;
                        var temperatureBefore = message.TemperatureBefore;
                        var temperatureAfter = message.TemperatureAfter;
                        var carno = message.CarNo;
                        var sensornum = message.SensorNum;

                        data_info.push([lng, lat, '车牌 : ' + carno +
                            (temperatureBefore > 0 ? '<br>排气温度 : ' + temperatureBefore + '度' : '') +
                            (temperatureAfter > 0 ? '<br>再生温度 : ' + temperatureAfter + '度' : '') +
                             (sensornum > 0 ? '<br>压力 ： ' + sensornum + 'Kpa' : '')
                        ]);
                        }   
                    var pointArr = [];
                    for (var i = 0; i < data_info.length; i++) {
                        var ggPoint = new BMap.Point(data_info[i][0], data_info[i][1]);
                        pointArr.push(ggPoint);
                    }
                    var convertor = new BMap.Convertor();
                    convertor.translate(pointArr, 1, 5, function (data) {
                        if (data.status == 0) {
                            for (var i = 0; i < data.points.length; i++) {
                                var marker = new BMap.Marker(data.points[i]);
                                map.addOverlay(marker);
                                map.setCenter(data.points[i]);
                                var content = data_info[i][2];
                                addClickHandler(content, marker);
                            }
                        }
                    });
                }

            },
            error: function (res) {
                console.log(res);
            }
        });
    }
    

  
    function initMap() {
        // 百度地图API功能
        map = new BMap.Map("allmap");
        map.centerAndZoom('哈尔滨', 15);
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

        var data_info = [
            [126.5424530000, 45.8074620000, '车牌：黑A68411<br>排气温度：104度<br>再生温度：143度<br>压力：3Kpa'],
            [126.5354110000, 45.8027370000, '车牌：黑AC1361<br>排气温度：120度<br>再生温度：153度<br>压力：4Kpa'],
            [126.5601320000, 45.8087680000, '车牌：黑LC1322<br>排气温度：101度<br>再生温度：150度<br>压力：5Kpa'],
        ];
        var pointArr = [];
        for (var i = 0; i < data_info.length; i++) {
            var ggPoint = new BMap.Point(data_info[i][0], data_info[i][1]);
            pointArr.push(ggPoint);
        }
        for (var i = 0; i < pointArr.length; i++) {
            var marker = new BMap.Marker(pointArr[i]);
            map.addOverlay(marker);
            map.setCenter(pointArr[i]);
            var content = data_info[i][2];

            addClickHandler(content, marker);
        }
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

    initMap();
    //query();
    //setInterval(function () {
    //    query();
    //}, 6000);
   
});
