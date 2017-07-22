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

	    initSelectData();
	    var table = initDataTable();
	    $("#btn-search").click(function () {
	        table.fnDraw();
	    });

	    //导入Json模板
	    $("#btn_import").click(function () {

	        $('#uploadfileinput').val('');
	        $('#upload').modal();
	    });

	    $('#upfileiframe').bind('load', function () {
	        try {
	            var str = upfileiframe.document.body.innerText;
	            if (str && str.length > 0) {
	                var resp = $.parseJSON(str);
	                if (resp.result == 0) {
	                     Dialog.primary("<p>导入成功</p>");
	                } else {
	                    Dialog.error("导入失败，请检查文件是否正确！");
	                }
	            }
	        } catch (e) {
	            Dialog.error("导入失败，请检查文件是否正确！");
	        }
	    });
	    $("#btn-uploadfile").click(function () {
	        var filename = $('#uploadfileinput')[0].value;
	        if (!filename || filename.length == 0) {
	            Dialog.warn('请选择文件');
	        } else {

	            $('#uploadform').submit();
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
	        "sAjaxSource": window.BASE_PATH + '/importcarinfo/query',
	        "fnServerData": retrieveData,
	        "columnDefs": [
                 {
                     "render": function (data, type, row) {
                         return row["ModifiedTime"] && row["ModifiedTime"].replace("T", " ");
                     },
                     "targets": 10
                 }

	        ],
	        "aoColumns": [{
	            "mData": "NO"
	        }, {
	            "mData": "Color"
	        }, {
	            "mData": "Category"
	        }, {
	            "mData": "Belong"
	        },
             {
                 "mData": "OriginalEmissionValues"
             },
             {
                 "mData": "ProductModel"
             },
             {
                 "mData": "ModifiedCompany"
             },
              {
                  "mData": "UserInfo"
              }, {
                  "mData": "IndividualCompany"
              }, {
                  "mData": "Telphone"
              }, {
                  "mData": "ModifiedTime"
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
	            aaData["aaData"].push(result.messages[i]);
	        }
	    }

	    return aaData;
	}
	function retrieveData(sSource, aoData, fnCallback) {
	    var Belong = $("#Belong").val();
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
	            Belong: Belong,
	            istart: iDisplayStart,
	            ilen: iDisplayLength,
	            StartDateTime: startDateTime,
                EndDateTime: endDateTime
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
	function initSelectData() {
	    //commonData.initSelectWithCarType("CarType", $("#Category"), '全部');
	    commonData.initSelectWithBelong("Belong", $("#Belong"), '全部');
	}
});