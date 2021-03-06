﻿define(function (require, exports, module) {

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

        $("#btn-add").click(function () {
            initSelectData();
            $("#modal-title").html("新增用户角色");
            $("#save-userrole-id").val('0');
            $("#save-username").val('');
            $('#save-userole-panel').modal('show');
            $('#save-userole-panel').on('shown.bs.modal', function (e) {
                $("#save-username").focus();
            })
        });


        $('#pclist tbody').on('click', '.edit', function () {//修改
            var row = JSON.parse($(this).attr("row"));

                $("#modal-title").html("编辑用户角色");
                $("#save-userrole-id").val(row.id);
                $("#save-username").val(row.username);
                $("#save-rolename").val(row.rolename);
                $('#save-userole-panel').modal('show');
                $('#save-userole-panel').on('shown.bs.modal', function (e) {
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
					            url: window.BASE_PATH + '/UserRoleInfo/Delete',
					            dataType: "json",
					            data: JSON.stringify({
					                userroleid: row.id
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
            saveUserRole();
        });
    }

    function initDataTable() {

        return $('#pclist').dataTable({
            'processing': true,
            'bStateSave': true,
            "bFilter": false,
            "bSort": false,
            "bServerSide": true,
            "sAjaxSource": window.BASE_PATH + '/UserRoleInfo/query',
            "fnServerData": retrieveData,
            "columnDefs": [
               {
                   "render": function (data, type, row) {
                       //编辑
                       var editHTML = "<div class='text-center'><a href='javascript:;' class='edit text-center' row='" + JSON.stringify(row) + "'>修改</a>";
                       var deleteHTML = "<a href='javascript:;' class='mlm delete text-center' row='" + JSON.stringify(row) + "'>删除</a></div>";

                       return editHTML + deleteHTML;
                   },
                   "targets": 6
               },
               {
                   "render": function (data, type, row) {
                       return row["createtime"] && row["createtime"].replace("T", " ");
                   },
                   "targets": 4
               },
                  {
                      "render": function (data, type, row) {
                          return row["lasttime"] && row["lasttime"].replace("T", " ");
                      },
                      "targets": 5
                  }

            ],
            "aoColumns": [{
                "mData": "id"
            }, {
                "mData": "username"
            }, {
                "mData": "rolename"
            }, {
                "mData": "page"
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
                //  if ("豫AC8760,豫AR0038,豫AL1098".indexOf(result.messages[i].CarNo) != -1) {
                aaData["aaData"].push(result.messages[i]);
                //  }

            }
        }

        return aaData;
    }
    function retrieveData(sSource, aoData, fnCallback) {
        var rolename = $("#RoleName").val();
        var username = $("#username").val();

        var iDisplayStart = Utils.getValueFromAOData(aoData, "iDisplayStart");
        var iDisplayLength = Utils.getValueFromAOData(aoData, "iDisplayLength");

        $.ajax({
            type: "get",
            contentType: "application/json; charset=utf-8",
            url: sSource,
            dataType: "json",
            data: {
                username:username,
                rolename: rolename,
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

    function initSelectData() {
        commonData.initSelectWithRoleName("RoleInfo", $("#RoleName"), '全部');
        commonData.initSelectWithRoleName("RoleInfo", $("#save-role-name"));
    }



    function saveUserRole() {
        var id = $("#save-userrole-id").val();
        var rolename = $("#save-role-name").val();
        var username = $("#save-username").val();
        if (Utils.isEmpty(username)) {
            Dialog.error("用户名不能为空！");
            return;
        }
        if (Utils.isEmpty(RoleName)) {
            Dialog.error("角色不能为空！");
            return;
        }


        BootstrapDialog.DEFAULT_TEXTS['OK'] = '确认';
        BootstrapDialog.DEFAULT_TEXTS['CANCEL'] = '取消';
        BootstrapDialog.DEFAULT_TEXTS['CONFIRM_TITLE'] = '确认';
        BootstrapDialog.confirm('您确认保存用户角色数据吗？', function (result) {
            if (result) {

                $.ajax({
                    type: "get",
                    contentType: "application/json; charset=utf-8",
                    url: window.BASE_PATH + '/UserRoleInfo/Save',
                    dataType: "json",
                    data: {
                        id: id,
                        rolename: rolename,
                        username: username
                    },
                    success: function (resp) {
                        if (resp.result == 0) {
                            $('#save-userole-panel').modal('hide');
                            Dialog.success("用户角色数据保存成功！");
                            var currentPage = parseInt($("#pclist_paginate li.active a").text()) - 1;
                            $("#pclist").dataTable().fnPageChange(currentPage, true);
                        } else {
                            Dialog.error("用户角色数据保存失败！");
                        }
                    },
                    error: function (resp) {
                        Dialog.error("用户角色数据保存失败！");
                    }
                });
            } else {
                $('#save-role-panel').modal('show');
            }
        });
    }


});