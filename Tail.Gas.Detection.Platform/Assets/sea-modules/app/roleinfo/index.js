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

        $("#btn-add").click(function () {
            initPageSelectData();
            $("#modal-title").html("新增角色");
            $("#save-role-id").val('0');
            $("#save-role-name").val('');
            $('#save-role-panel').modal('show');
            $('#save-role-panel').on('shown.bs.modal', function (e) {
                $("#save-role-name").focus();
            })
        });
        //新增 编辑 全选 所属 bu
        $("#Role_Page_ALL").click(function () {
            $("#Role_Page_List input[type='checkbox']").prop("checked", $("#Role_Page_ALL").prop("checked"));
        });

        $('#pclist tbody').on('click', '.edit', function () {//修改
            var row = JSON.parse($(this).attr("row"));
            initPageSelectData(function () {
               
                $("#modal-title").html("编辑角色");
                $("#save-role-id").val(row.id);
                $("#save-role-name").val(row.rolename);
                $('#save-role-panel').modal('show');
                $('#save-role-panel').on('shown.bs.modal', function (e) {
                    $("#save-role-name").focus();
                })
            });
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
					            url: window.BASE_PATH + '/roleinfo/Delete',
					            dataType: "json",
					            data: JSON.stringify({
					                roleid: row.id
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

    function initPageSelectData(callback) {
        $("#Role_Page_ALL").prop("checked", false);
        $("#Role_Page_List").html("");
        commonData.initCheckBoxWithCommonData("PageInfo", $("#Role_Page_List"), callback);
    }
    function initDataTable() {

        return $('#pclist').dataTable({
            'processing': true,
            'bStateSave': true,
            "bFilter": false,
            "bSort": false,
            "bServerSide": true,
            "sAjaxSource": window.BASE_PATH + '/roleinfo/query',
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


        var iDisplayStart = Utils.getValueFromAOData(aoData, "iDisplayStart");
        var iDisplayLength = Utils.getValueFromAOData(aoData, "iDisplayLength");

        $.ajax({
            type: "get",
            contentType: "application/json; charset=utf-8",
            url: sSource,
            dataType: "json",
            data: {
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
    }

    //获取所选bu 返回结果 example "1,2,45,123"
    function getSelectPage() {
        var selitems = $("#Role_Page_List input[type='checkbox']:checked");
        if (selitems && selitems.length > 0) {
            var authbus = "";
            selitems.each(function () {
                var $this = $(this);
                authbus += $this.val() + ",";
            });
            authbus = authbus.substr(0, authbus.length - 1);
            return authbus;
        } else {
            return "";
        }
    }

    function saveUserRole() {
        var id = $("#save-role-id").val();
        var rolename = $("#save-role-name").val();
        var pages = getSelectPage();
        if (Utils.isEmpty(RoleName)) {
            Dialog.error("角色不能为空！");
            return;
        }

        if (Utils.isEmpty(pages)) {
            Dialog.error("请分配页面！");
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
                    url: window.BASE_PATH + '/roleinfo/Save',
                    dataType: "json",
                    data: {
                        id: id,
                        rolename: rolename,
                        page: pages
                    },
                    success: function (resp) {
                        if (resp.result == 0) {
                            $('#save-role-panel').modal('hide');
                            Dialog.success("角色数据保存成功！");
                            var currentPage = parseInt($("#pclist_paginate li.active a").text()) - 1;
                            $("#pclist").dataTable().fnPageChange(currentPage, true);
                        } else {
                            Dialog.error("角色数据保存失败！");
                        }
                    },
                    error: function (resp) {
                        Dialog.error("角色数据保存失败！");
                    }
                });
            } else {
                $('#save-role-panel').modal('show');
            }
        });
    }


});