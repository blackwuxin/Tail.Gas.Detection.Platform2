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
    public class UserRoleInfoController : BaseController
    {
        [AuthorizeFilterAttribute]
        public ActionResult Index()
        {
            return View();
        }

        public string Delete(int userroleid)
        {
            JObject joResult = new JObject();
            try
            {
                UserRoleInfoDao.DeleteUserRole(userroleid);
                joResult["result"] = 0;
            }
            catch (Exception ex)
            {
                joResult["result"] = -1;
            }
            return joResult.ToString();
        }

        public string Save(UserRoleInfo userrole)
        {
            JObject joResult = new JObject();
            try
            {
                if (userrole.id == 0)//add
                {
                    userrole.createtime = DateTime.Now;
                    userrole.lasttime = DateTime.Now;
                    UserRoleInfoDao.InsertUserRoleInfo(userrole);
                }
                else//edit
                {
                    UserRoleInfoDao.UpdateUserRole(userrole);
                }
                joResult["result"] = 0;

       
            }
            catch (Exception ex)
            {
                joResult["result"] = -1;
            }
           return joResult.ToString();
        }

        public string query(string username ,string rolename, int istart, int ilen)
        {

            JObject joResult = new JObject();
            JArray messageList = new JArray();
            long totalCount = 0;

            UserRoleInfoDao.GetUserRoleList(username, rolename, ilen, istart / ilen + 1, out totalCount, ref messageList);
            joResult["messages"] = messageList;
            joResult["total-records"] = totalCount;
            return joResult.ToString();
        }
    }
}
