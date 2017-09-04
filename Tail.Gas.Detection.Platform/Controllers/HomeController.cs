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

namespace Tail.Gas.Detection.Platform.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.Session["username"] != null)
            {
                return RedirectToAction("index","carstatusinfo");
            }
            else
            {
                return View();
            }
            
        }
        public ActionResult Forbidden()
        {
            return View();
        }
        public string login(string username, string password,string usertype)
        {
            //var s = "[{'username':'user527','pwd':'admin'},{'username':'user528','pwd':'admin'},{'username':'user529','pwd':'admin'}]";
            //JArray users = (JArray)JsonConvert.DeserializeObject(ConfigurationManager.AppSettings["users"]);
           // username = "user527";
           // password = "admin";
            JObject joResult = new JObject();

           var b =  UserInfoDao.CheckUserInfo(username, password, usertype);
           if (b!="error")
           {
               
               System.Web.HttpContext.Current.Session["username"] = username;
               System.Web.HttpContext.Current.Session["usertype"] = usertype;
               System.Web.HttpContext.Current.Session["page"] = b;
               joResult["result"] = 0;
           }
           else
           {
               joResult["result"] = 1;
           }
           // foreach (JObject items in users)  
           // {
           //     if (items["username"].ToString() == username && items["pwd"].ToString() == password && items["usertype"].ToString() == usertype)
           //     {
           //         System.Web.HttpContext.Current.Session["username"] = username;
           //         System.Web.HttpContext.Current.Session["usertype"] = usertype;
           //         joResult["result"] = 0;
           //         return joResult.ToString();
           //     }
           //}  

            
            return joResult.ToString();
        }
    }
}
