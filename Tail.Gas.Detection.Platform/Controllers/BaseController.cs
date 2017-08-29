using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Tail.Gas.Detection.Platform.Controllers
{
    public class BaseController : Controller
    {
        public const byte RESULT_SUCCESS = 0;
        public const byte RESULT_ERROR = 1;


        public static ServiceResponse CreateSuccessResponse()
        {
            return CreateSuccessResponse(default(byte), default(object));
        }

        public static ServiceResponse CreateSuccessResponse(object data)
        {
            return CreateSuccessResponse(default(byte), data);
        }

        public static ServiceResponse CreateSuccessResponse(byte resultCode, object data)
        {
            return new ServiceResponse() { Result = RESULT_SUCCESS, ResultCode = resultCode, Body = data };
        }

        public static ServiceResponse CreateErrorResponse()
        {
            return CreateErrorResponse(default(byte), default(string));
        }

        public static ServiceResponse CreateErrorResponse(string errorMsg)
        {
            return CreateErrorResponse(default(byte), errorMsg);
        }
        public static ServiceResponse CreateErrorResponse(byte resultCode, string errorMsg)
        {
            return new ServiceResponse() { Result = RESULT_ERROR, ResultCode = resultCode, ResultMessage = errorMsg };
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
    
            ViewBag.ApplicationPath = ApplicationPath;
            ViewBag.Menus = GetMenusString();
            ViewBag.ToolKit = GetToolKitString();
        }
        private string GetMenusString()
        {
            StringBuilder sbMenus = new StringBuilder();
            string activeClass = " class='active'";
            string url = Request.RawUrl.ToLower();

            if (!url.Contains("home")&& !url.Equals("/myapp"))
            {
                sbMenus.Append(string.Format("<li role='presentation' {0}><a href='{1}/carstatusinfo/Index' target=''>状态正常</a></li>",
                   url.Contains("/carstatusinfo/index")||url.Equals("/myapp/carstatusinfo") ? activeClass : "", ApplicationPath));
                sbMenus.Append(string.Format("<li role='presentation' {0}><a href='{1}/carstatusinfo/error' target=''>状态异常</a></li>",
                   url.Contains("/carstatusinfo/error") ? activeClass : "", ApplicationPath));
                sbMenus.Append(string.Format("<li role='presentation' {0}><a href='{1}/map/Index' target=''>地图</a></li>",
                 url.Contains("/map/Index") ? activeClass : "", ApplicationPath));
                sbMenus.Append(string.Format("<li role='presentation' {0}><a href='{1}/carstatusinfo/querycarstatus' target=''>车辆状态查询</a></li>",
                     url.Contains("/carstatusinfo/querycarstatus") ? activeClass : "", ApplicationPath));

            }
            if ("监管人员".Equals(System.Web.HttpContext.Current.Session["usertype"]) || "管理员".Equals(System.Web.HttpContext.Current.Session["usertype"]))
            {
                sbMenus.Append(string.Format("<li role='presentation' {0}><a href='{1}/importcarinfo/' target=''>导入车辆信息</a></li>",
  url.Contains("/importcarinfo") ? activeClass : "", ApplicationPath));

                sbMenus.Append(string.Format("<li role='presentation' {0}><a href='{1}/carstatusinfo/download' target=''>车辆状态下载</a></li>",
                       url.Contains("/carstatusinfo/download") ? activeClass : "", ApplicationPath));
            }
            if ("管理员".Equals(System.Web.HttpContext.Current.Session["usertype"]))
            {
                sbMenus.Append(string.Format("<li role='presentation' {0}><a href='{1}/roleinfo/index' target=''>角色管理</a></li>",
                    url.Contains("/roleinfo/index") ? activeClass : "", ApplicationPath));
            }
           
            return sbMenus.ToString();
        }
        /// <summary>
        /// 工具栏
        /// </summary>
        /// <returns></returns>
        private string GetToolKitString()
        {
            string url = Request.RawUrl.ToLower();

            if (!url.Contains("home") && !url.Equals("/myapp"))
             {
                    StringBuilder sbToolKit = new StringBuilder();
                    sbToolKit.Append(string.Format("<a id=\"logout\" href=\"javascript:;\" class=\"hd-toolkit-item\"><i class=\"icon-user icon-white\"></i>注销</a>"));

                    string script = @"seajs.use('jquery', function($){
                                            $('.hd-toolkit-item').remove(); 
                                            $('.hd-toolkit').prepend('" + sbToolKit.ToString() + "'); })";

                    return script;
             }else{
                 return "";
             }

        
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public string Logout()
        {
            System.Web.HttpContext.Current.Session["username"] = null;
            System.Web.HttpContext.Current.Session["usertype"] = null;

            JObject joRow = new JObject();
            joRow["result"] = "success";
   
            return joRow.ToString();
        }
        /// <summary>
        /// 返回不以斜杠结尾的应用程序路径
        /// </summary>
        protected string ApplicationPath
        {
            get
            {
                string applicationPath = Request.ApplicationPath;
                return applicationPath.TrimEnd('/');
            }
        }
    }
    public class ServiceResponse
    {
        [JsonProperty(PropertyName = "result")]
        public byte Result { get; set; }

        [JsonProperty(PropertyName = "resultCode")]
        public byte ResultCode { get; set; }

        [JsonProperty(PropertyName = "resultMessage")]
        public string ResultMessage { get; set; }

        [JsonProperty(PropertyName = "data")]
        public object Body { get; set; }
    }

    public class ServiceDataResponse : ServiceResponse
    {
        public long iTotalRecords { get; set; }
        public long iTotalDisplayRecords { get; set; }
        public List<object> aaData { get; set; }

    }
}
