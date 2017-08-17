define(function (require, exports, module) {

    var $ = require('jquery');
    //var BMap = require('BMap');
    var JSON = require('json');
    var commonData = require("commondata");
    var belong = '哈尔滨';
  //  var lastpoint = localStorage.getItem('lastpoint');
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

                    var data_info = [];
                    for (var i = 0, l = res.messages.length; i < l; i++) {
                        var message = res.messages[i];

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

                        if ((message.PositionXDegree === 0 && message.PositionXM === 0 && message.PositionXS === 0)
                            || (message.PositionYDegree === 0 && message.PositionYM === 0 && message.PositionYS === 0)
                            || message.PositionXM > 60 || message.PositionXS >6000 
                            || message.PositionYM > 60 || message.PositionYS>6000)

                        {
                          //  if (!lastpoint) {
                                continue;
                         //   }
                         //   point = lastpoint;
                        } else {
                         //   lastpoint = point;
                          //  localStorage.setItem('lastpoint', lastpoint);
                        }

                        

                        data_info.push(point);
                        //data_info.push([lng, lat, '车牌 : ' + carno +
                        //    (temperatureBefore > 0 ? '<br>排气温度 : ' + temperatureBefore + '度' : '') +
                        //    (temperatureAfter > 0 ? '<br>再生温度 : ' + temperatureAfter + '度' : '') +
                        //     (sensornum > 0 ? '<br>压力 : ' + sensornum + 'Kpa' : '') +
                        //     '<br>时间 : ' + data_lasttime
                        //]);
                    }   
                    var pointArr = [];
                    for (var i = 0; i < data_info.length; i++) {
                        var ggPoint = new BMap.Point(data_info[i][0], data_info[i][1]);
                        pointArr.push(ggPoint);
                    }

                    var convertor = new BMap.Convertor();
                    for (var i = 0; i < pointArr.length; i = i + 10) {
                        map.clearOverlays();
                        var obj = pointArr.slice(i,i+10);
                        convertor.translate(obj, 1,5, function (data) {
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

        function myFun(result) {
            var cityName = result.name;
            commonData.initSelectWithCity("Belong", $("#Belong"), cityName);
            // map.centerAndZoom(cityName,10);
            //alert("当前定位城市:" + cityName);

        }
        var myCity = new BMap.LocalCity();
        myCity.get(myFun);
        $("#Belong").change(function (value) {

            map.centerAndZoom(value.target.value, 10);
            belong = value.target.value;
        });
        
        //var data_info = [
        //[127.773868, 45.850577],
        //[127.773814,45.850435],
        //[127.771227,45.850435],
        //[127.767131,45.849883],
        //[127.762388,45.849481],
        //[127.758507,45.848878],
        //[127.75592,45.847171 ],
        //[127.753045,45.844861],
        //[127.749452,45.841646],
        //[127.746003,45.838933],
        //[127.744422,45.837326],
        //[127.741403,45.835015],
        //[127.738816,45.832805],
        //[127.73551,45.830092 ],
        //[127.731055,45.827479],
        //[127.730767,45.827379],
        //[127.724156,45.825671],
        //[127.716969,45.823259],
        //[127.710214,45.821249],
        //[127.704034,45.818737],
        //[127.698141,45.816927],
        //[127.690811,45.814817],
        //[127.683768,45.812706],
        //[127.676725,45.810294],
        //[127.642374,45.797326],
        //[127.635475,45.794712],
        //[127.628864,45.7925  ],
        //[127.61909,45.790991 ],
        //[127.607448,45.789081],
        //[127.594656,45.786668],
        //[127.594656,45.786668],
        //[127.588907,45.784958],
        //[127.583014,45.782746],
        //[127.576834,45.780634],
        //[127.571085,45.778823],
        //[127.566773,45.777315],
        //[127.561311,45.7747  ],
        //[127.556137,45.773292],
        //[127.552256,45.771481],
        //[127.547657,45.76957 ],
        //[127.542339,45.768564],
        //[127.53774,45.767357 ],
        //[127.530266,45.766351],
        //[127.52351,45.765244 ],
        //[127.519917,45.76454 ],
        //[127.517617,45.763937],
        //[127.513018,45.762729],
        //[127.509137,45.761925],
        //[127.503963,45.760918],
        //[127.500083,45.760013],
        //[127.494908,45.758806],
        //[127.494765,45.758806],
        //[127.492034,45.7579  ],
        //[127.487003,45.756793],
        //[127.482116,45.75458 ],
        //[127.477373,45.752568],
        //[127.473493,45.751159],
        //[127.470474,45.750455],
        //[127.465444,45.750153],
        //[127.45912,45.750958 ],
        //[127.453802,45.751662],
        //[127.448484,45.752266],
        //[127.444459,45.752869],
        //[127.440004,45.753272],
        //[127.433824,45.754479]
        //]
        //var data_info = [
        //    [126.5424530000, 45.8074620000, '车牌：黑A68411<br>排气温度：104度<br>再生温度：143度<br>压力：3Kpa'],
        //    [126.5354110000, 45.8027370000, '车牌：黑AC1361<br>排气温度：120度<br>再生温度：153度<br>压力：4Kpa'],
        //    [126.5601320000, 45.8087680000, '车牌：黑LC1322<br>排气温度：101度<br>再生温度：150度<br>压力：5Kpa'],
        //];
        //var pointArr = [];
        //for (var i = 0; i < data_info.length; i++) {
        //    var ggPoint = new BMap.Point(data_info[i][0], data_info[i][1]);
        //    pointArr.push(ggPoint);
        //}
        //for (var i = 0; i < pointArr.length; i++) {
        //    var marker = new BMap.Marker(pointArr[i]);
        //    map.addOverlay(marker);
        //    map.setCenter(pointArr[i]);
        //    var content = data_info[i][2];

        //    addClickHandler(content, marker);
        //}
        //map.centerAndZoom(pointArr[0],10);
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
    query();
    setInterval(function () {
        query();
    }, 60000);
   
});
