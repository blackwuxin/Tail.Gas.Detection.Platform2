using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tail.Gas.Detection.Platform.Authorize;

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
            JArray users = (JArray)JsonConvert.DeserializeObject(ConfigurationManager.AppSettings["users"]);
           // username = "user527";
           // password = "admin";
            JObject joResult = new JObject();
            foreach (JObject items in users)  
            {
                if (items["username"].ToString() == username && items["pwd"].ToString() == password && items["usertype"].ToString() == usertype)
                {
                    System.Web.HttpContext.Current.Session["username"] = username;
                    System.Web.HttpContext.Current.Session["usertype"] = usertype;
                    joResult["result"] = 0;
                    return joResult.ToString();
                }
           }  

            joResult["result"] = 1;
            return joResult.ToString();
        }
    }
}
