define(function (require, exports, module) {

    var $ = require('jquery');
    //var BMap = require('BMap');
    var JSON = require('json');
    var belong = '哈尔滨';
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

            }
         
        },
        error: function (res) {
            console.log(res);
        }
    });

    // 百度地图API功能
    map = new BMap.Map("allmap");
    map.centerAndZoom('哈尔滨', 10);
    //map.centerAndZoom('长兴', 15);

    var data_info = [
        [126.616666, 45.633333, '车牌：黑A68411<br>排气温度：61度<br>再生温度：75度<br>压力：3Kpa'],
        [126.610130067, 45.781155467, '车牌：黑A68411<br>排气温度：61度<br>再生温度：75度<br>压力：3Kpa'],
        [126.2802028656, 45.6416665777, '车牌：黑A68411<br>排气温度：61度<br>再生温度：75度<br>压力：3Kpa'],
    ];
    var opts = {
        width: 250,     // 信息窗口宽度
        height: 120,     // 信息窗口高度
        title: "<h4>车辆详情</h4>", // 信息窗口标题
        enableMessage: true//设置允许信息窗发送短息
    };
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
    function addClickHandler(content, marker) {
        marker.addEventListener("click", function (e) {
            openInfo(content, e)
        }
		);
    }
    function openInfo(content, e) {
        var p = e.target;
        var point = new BMap.Point(p.getPosition().lng, p.getPosition().lat);
        var infoWindow = new BMap.InfoWindow(content, opts);  // 创建信息窗口对象
        map.openInfoWindow(infoWindow, point); //开启信息窗口
    }

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
  
   
});
