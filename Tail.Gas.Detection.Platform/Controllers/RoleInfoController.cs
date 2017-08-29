using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tail.Gas.Detection.Platform.Authorize;
using Tail.Gas.Detection.Platform.Dao;
using Tail.Gas.Detection.Platform.Models;

namespace Tail.Gas.Detection.Platform.Controllers
{
    public class RoleInfoController : BaseController
    {
        //[AuthorizeFilterAttribute]
        public ActionResult Index()
        {
            return View();
        }

        public string Delete(int roleid)
        {
            JObject joResult = new JObject();
            try
            {
                RoleInfoDao.DeleteRoleInfo(roleid);
                joResult["result"] = 0;
            }
            catch (Exception ex)
            {
                joResult["result"] = -1;
            }
            return joResult.ToString();
        }

        public string Save(RoleInfo role)
        {
            JObject joResult = new JObject();
            try
            {
                if (role.id == 0)//add
                {
                    role.createtime = DateTime.Now;
                    role.lasttime = DateTime.Now;
                    RoleInfoDao.InsertRoleInfo(role);
                }
                else//edit
                {
                    RoleInfoDao.UpdateRoleInfo(role);
                }
                joResult["result"] = 0;

       
            }
            catch (Exception ex)
            {
                joResult["result"] = -1;
            }
           return joResult.ToString();
        }

        public string query(string rolename, int istart, int ilen)
        {

            JObject joResult = new JObject();
            JArray messageList = new JArray();
            long totalCount = 0;

            RoleInfoDao.GetRoleList(rolename, ilen, istart / ilen + 1, out totalCount, ref messageList);
            joResult["messages"] = messageList;
            joResult["total-records"] = totalCount;
            return joResult.ToString();
        }
    }
}
