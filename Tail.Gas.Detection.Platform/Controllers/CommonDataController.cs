using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tail.Gas.Detection.Platform.Models;

namespace Tail.Gas.Detection.Platform.Controllers
{
    public class CommonDataController : Controller
    {
        public readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static cardb2Entities1 cardb = new cardb2Entities1();
        public string GetCommonData(string type)
        {
            JObject jobject = new JObject();
            JArray messageList = new JArray();
            try
            {
                if ("Province".Equals(type))
                {
                    foreach (var item in cardb.CarNoProvince)
                    {
                        JObject joRow = new JObject();
                        joRow["CarNo"] = item.CarNo;
                        messageList.Add(joRow);
                    }
                }
                else if ("Belong".Equals(type))
                {
                    foreach (var item in cardb.CarNoBlong.Select(a => a.Belong).Distinct())
                    {
                        JObject joRow = new JObject();
                        joRow["Belong"] = item;
                        messageList.Add(joRow);
                    }

                }
                else if ("CarType".Equals(type))
                {
                    foreach (var item in cardb.CarType)
                    {
                        JObject joRow = new JObject();
                        joRow["Name"] = item.Name;
                        messageList.Add(joRow);
                    }
                }
                else if("RoleInfo".Equals(type))
                {
                    foreach (var item in cardb.RoleInfo)
                    {
                        JObject joRow = new JObject();
                        joRow["Name"] = item.rolename;
                        messageList.Add(joRow);
                    }
                }
                else if ("PageInfo".Equals(type))
                {
                    foreach (var item in cardb.PageInfo)
                    {
                        JObject joRow = new JObject();
                        joRow["pagename"] = item.pagename;
                        joRow["pagetype"] = item.pagetype;
                        messageList.Add(joRow);
                    }
                }
                jobject["result"] = 0;
                jobject["data"] = messageList;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
           
            return jobject.ToString();
        }

    }
}
