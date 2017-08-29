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
    public class UserInfoController : BaseController
    {
        //[AuthorizeFilterAttribute]
        public ActionResult Index()
        {
            return View();
        }

        public string Delete(int userid)
        {
            JObject joResult = new JObject();
            try
            {
                UserInfoDao.DeleteUserInfo(userid);
                joResult["result"] = 0;
            }
            catch (Exception ex)
            {
                joResult["result"] = -1;
            }
            return joResult.ToString();
        }

        public string Save(UserInfo user)
        {
            JObject joResult = new JObject();
            try
            {
                if (user.id == 0)//add
                {
                    user.createtime = DateTime.Now;
                    user.lasttime = DateTime.Now;
                    UserInfoDao.InsertUserInfo(user);

                }
                else//edit
                {
                    UserInfoDao.UpdateUserInfo(user);
                }
                joResult["result"] = 0;

       
            }
            catch (Exception ex)
            {
                joResult["result"] = -1;
            }
           return joResult.ToString();
        }

        public string query(string username, int istart, int ilen)
        {

            JObject joResult = new JObject();
            JArray messageList = new JArray();
            long totalCount = 0;

            UserInfoDao.GetUserList(username, ilen, istart / ilen + 1, out totalCount, ref messageList);
            joResult["messages"] = messageList;
            joResult["total-records"] = totalCount;
            return joResult.ToString();
        }
    }
}
