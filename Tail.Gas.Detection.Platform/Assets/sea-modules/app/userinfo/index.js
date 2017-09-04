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
  
        var table = initDataTable();
        $("#btn-search").click(function () {
            table.fnDraw();
        });

        $("#btn-add").click(function () {

            $("#modal-title").html("新增用户信息");
            $("#save-user-id").val('0');
            $("#save-username").val('');
            $("#save-password").val('');
            $('#save-user-panel').modal('show');
            $('#save-user-panel').on('shown.bs.modal', function (e) {
                $("#save-username").focus();
            })
        });


        $('#pclist tbody').on('click', '.edit', function () {//修改
            var row = JSON.parse($(this).attr("row"));
         
               
                $("#modal-title").html("编辑用户信息");
                $("#save-user-id").val(row.id);
                $("#save-username").val(row.username);
                $('#save-user-panel').modal('show');
                $('#save-user-panel').on('shown.bs.modal', function (e) {
                    $("#save-username").focus();
                })
       
        }).on('click', '.delete', function () {//删除

            var row = JSON.parse($(this).attr("row"));

            BootstrapDialog.show({
                title: '确认',
                message: "您确认删除用户角色信息吗？",
                buttons: [
					{
					    id: 'btn-save-confirm',
					    label: '确认',
					    cssClass: 'btn-primary',
					    action: function (dialog) {
					        var $button = this;
					        $button.disable();
					        $button.spin();
					        dialog.setClosable(false);

					        $.ajax({
					            type: "POST",
					            contentType: "application/json; charset=utf-8",
					            url: window.BASE_PATH + '/userinfo/Delete',
					            dataType: "json",
					            data: JSON.stringify({
					                userid: row.id
					            }),
					            success: function (resp) {
					                if (resp.result == 0) {
					                    setTimeout(function () {
					                        dialog.close();
					                        Dialog.success("用户角色信息删除成功！");
					                    }, 100);
					                    var currentPage = parseInt($("#pclist_paginate li.active a").text()) - 1;
					                    $("#pclist").dataTable().fnPageChange(currentPage, true);
					                } else {
					                    dialog.close();
					                    Dialog.error("用户角色信息删除失败，请重试！");
					                }
					            },
					            error: function (resp) {
					                dialog.close();
					                Dialog.error("用户角色信息删除失败，请重试！");
					            }
					        });
					    }
					},
					{
					    id: 'btn-cancel',
					    label: '取消',
					    action: function (dialog) {
					        dialog.close();
					    }
					}]
            });
        });

        $("#btn-save").click(function () {
            saveUser();
        });
    }


    function initDataTable() {

        return $('#pclist').dataTable({
            'processing': true,
            'bStateSave': true,
            "bFilter": false,
            "bSort": false,
            "bServerSide": true,
            "sAjaxSource": window.BASE_PATH + '/userinfo/query',
            "fnServerData": retrieveData,
            "columnDefs": [
               {
                   "render": function (data, type, row) {
                       //编辑
                       var editHTML = "<div class='text-center'><a href='javascript:;' class='edit text-center' row='" + JSON.stringify(row) + "'>修改</a>";
                       var deleteHTML = "<a href='javascript:;' class='mlm delete text-center' row='" + JSON.stringify(row) + "'>删除</a></div>";

                       return editHTML + deleteHTML;
                   },
                   "targets": 5
               },
               {
                   "render": function (data, type, row) {
                       return row["createtime"] && row["createtime"].replace("T", " ");
                   },
                   "targets": 3
               },
                  {
                      "render": function (data, type, row) {
                          return row["lasttime"] && row["lasttime"].replace("T", " ");
                      },
                      "targets": 4
                  }

            ],
            "aoColumns": [{
                "mData": "id"
            }, {
                "mData": "username"
            }, {
                "mData": "password"
            }, {
                "mData": "createtime"
            }, {
                "mData": "lasttime"
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
        var username = $("#username").val();


        var iDisplayStart = Utils.getValueFromAOData(aoData, "iDisplayStart");
        var iDisplayLength = Utils.getValueFromAOData(aoData, "iDisplayLength");

        $.ajax({
            type: "get",
            contentType: "application/json; charset=utf-8",
            url: sSource,
            dataType: "json",
            data: {
                username: username,
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





    function saveUser() {
        var id = $("#save-user-id").val();
        var username = $("#save-username").val();
        var password = $("#save-password").val();
        if (Utils.isEmpty(username)) {
            Dialog.error("用户名不能为空！");
            return;
        }

        if (Utils.isEmpty(password)) {
            Dialog.error("密码不能为空！");
            return;
        }

        BootstrapDialog.DEFAULT_TEXTS['OK'] = '确认';
        BootstrapDialog.DEFAULT_TEXTS['CANCEL'] = '取消';
        BootstrapDialog.DEFAULT_TEXTS['CONFIRM_TITLE'] = '确认';
        BootstrapDialog.confirm('您确认保存角色数据吗？', function (result) {
            if (result) {

                $.ajax({
                    type: "get",
                    contentType: "application/json; charset=utf-8",
                    url: window.BASE_PATH + '/userinfo/Save',
                    dataType: "json",
                    data: {
                        id: id,
                        username: username,
                        password: password
                    },
                    success: function (resp) {
                        if (resp.result == 0) {
                            $('#save-user-panel').modal('hide');
                            Dialog.success("用户信息保存成功！");
                            var currentPage = parseInt($("#pclist_paginate li.active a").text()) - 1;
                            $("#pclist").dataTable().fnPageChange(currentPage, true);
                        } else if(resp.result == -2) {
                            Dialog.error("用户名重复！");
                        } else {
                            Dialog.error("用户信息保存失败！");
                        }
                    },
                    error: function (resp) {
                        Dialog.error("用户信息保存失败！");
                    }
                });
            } else {
                $('#save-user-panel').modal('show');
            }
        });
    }


});