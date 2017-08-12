using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.IO;
using System.Web.Mvc;
using Tail.Gas.Detection.Platform.Authorize;
using Tail.Gas.Detection.Platform.Dao;

namespace Tail.Gas.Detection.Platform.Controllers
{
    public class MapController : BaseController
    {
       

        // GET: /Map
        [AuthorizeFilterAttribute]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult map2()
        {
            return View("map2");
        
        }
        public ActionResult Path()
        {
            return View("Path");
        }
        public string query(string Belong)
        {
            JObject joResult = new JObject();
            JArray messageList = new JArray();
            CarStatusInfoDao2.GetMapList(Belong, ref messageList);
            joResult["messages"] = messageList;
            return joResult.ToString();
        }
        public string query2(string Belong)
        {
            JObject joResult = new JObject();
            JArray messageList = new JArray();
            CarStatusInfoDao2.GetMapList2(Belong, ref messageList);
            joResult["messages"] = messageList;
            return joResult.ToString();
        }

        public string getmappath(string CarNo, string startDateTime, string endDateTime)
        {

            JObject joResult = new JObject();
            JArray messageList = new JArray();
            CarStatusInfoDao2.GetStatus(CarNo, startDateTime, endDateTime, ref messageList);
            joResult["messages"] = messageList;
            return joResult.ToString();

        }

       
      
    }
}
