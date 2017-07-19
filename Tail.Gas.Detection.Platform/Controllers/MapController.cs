using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tail.Gas.Detection.Platform.Authorize;
using Tail.Gas.Detection.Platform.Dao;
using Tail.Gas.Detection.Platform.Models;

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

        public string query(string Belong)
        {
            JObject joResult = new JObject();
            JArray messageList = new JArray();
            CarStatusInfoDao2.GetMapList(Belong, ref messageList);
            joResult["messages"] = messageList;
            return joResult.ToString();
        }

    }
}
