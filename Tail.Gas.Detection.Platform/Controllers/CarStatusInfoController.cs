using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tail.Gas.Detection.Platform.Authorize;
using Tail.Gas.Detection.Platform.Dao;
using Tail.Gas.Detection.Platform.Models;

namespace Tail.Gas.Detection.Platform.Controllers
{
    public class CarStatusInfoController : BaseController
    {
        public readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: /CarStatusInfo/
        [AuthorizeFilterAttribute]
        public ActionResult Index()
        {
            return View("NormalList");
            
        }
        [AuthorizeFilterAttribute]
        public ActionResult error()
        {
            return View("ErrorList");
        }
        [AuthorizeFilterAttribute]
        public ActionResult download()
        {
            return View("DownLoad");
        }
        [AuthorizeFilterAttribute]
        public ActionResult querycarstatus()
        {
            return View("Query");
        }
        public string query(string PageUrl, string CarNo,string Category, string Belong, int istart, int ilen)
        {

           JObject joResult = new JObject();
            JArray messageList = new JArray();
            long totalCount = 0;
            if (!string.IsNullOrEmpty(PageUrl) && PageUrl.ToLower().Contains("carstatusinfo/index") || PageUrl.Equals("/myapp/carstatusinfo"))
            {
                CarStatusInfoDao2.GetNormalListByPage(CarNo,Category,
                    Belong,ilen, istart / ilen + 1, out totalCount, ref messageList);
            }
            else if (!string.IsNullOrEmpty(PageUrl) && PageUrl.ToLower().Contains("carstatusinfo/error"))
            {
                CarStatusInfoDao2.GetErrorListByPage(CarNo, Category,
                    Belong, ilen, istart / ilen + 1, out totalCount, ref messageList);
            }
            joResult["messages"] = messageList;
            joResult["total-records"] = totalCount;
            return joResult.ToString();
        }
        public string queryfilename(string CarNo, string startDateTime, string endDateTime)
        {

            JObject joResult = new JObject();
            JArray messageList = new JArray();
            CarStatusInfoDao2.GetStatus(CarNo, startDateTime, endDateTime, ref messageList);
            joResult["messages"] = messageList;

            if (messageList.Count == 0)
            {
                return joResult.ToString();
            }
            // WriteToExcel("../filedemo/" + new DateTime() + ".xls");
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string filePath = Request.PhysicalApplicationPath + "\\filedemo\\" + messageList[0]["CarNo"] + "_" + Convert.ToInt64(ts.TotalSeconds).ToString() + ".xls";
            string filePath2 = Request.ApplicationPath.TrimEnd('/') + "/filedemo/" + messageList[0]["CarNo"] + "_" + Convert.ToInt64(ts.TotalSeconds).ToString() + ".xls";
            //string filePath = Request.ApplicationPath.TrimEnd('/') + "/filedemo/"  + "test.xls";
            //创建工作薄  
            IWorkbook wb;
            string extension = System.IO.Path.GetExtension(filePath);
            //根据指定的文件格式创建对应的类
            if (extension.Equals(".xls"))
            {
                wb = new HSSFWorkbook();
            }
            else
            {
                wb = new XSSFWorkbook();
            }

            //创建一个表单
            ISheet sheet = wb.CreateSheet("Sheet0");

            //测试数据
            int rowCount = messageList.Count, columnCount = 16;
            object[,] data = new object[rowCount, columnCount];
            // {"车牌","车辆颜色","类别","归属地","排气温度","再生温度","压力","转速","状态","经度度","经度分","经度秒","纬度度","纬度分","纬度秒","时间"},

            data[0, 0] = "车牌";
            data[0, 1] = "车辆颜色";
            data[0, 2] = "类别";
            data[0, 3] = "归属地";
            data[0, 4] = "排气温度";
            data[0, 5] = "再生温度";
            data[0, 6] = "压力";
            data[0, 7] = "转速";
            data[0, 8] = "状态";
            data[0, 9] = "经度度";
            data[0, 10] = "经度分";
            data[0, 11] = "经度秒";
            data[0, 12] = "纬度度";
            data[0, 13] = "纬度分";
            data[0, 14] = "纬度秒";
            data[0, 15] = "时间";

            for (int i = 1; i < rowCount; i++)
            {
                JObject jo = (JObject)messageList[i];
                data[i, 0] = jo["CarNo"];
                data[i, 1] = jo["Color"];
                data[i, 2] = jo["Category"];
                data[i, 3] = jo["Belong"];
                data[i, 4] = jo["EngineSpeed"];
                data[i, 5] = jo["TemperatureBefore"];
                data[i, 6] = jo["TemperatureAfter"];
                data[i, 7] = jo["SensorNum"];
                data[i, 8] = jo["SystemStatus"];

                data[i, 9] = jo["PositionXDegree"];
                data[i, 10] = jo["PositionXM"];
                data[i, 11] = jo["PositionXS"];
                data[i, 12] = jo["PositionYDegree"];
                data[i, 13] = jo["PositionYM"];
                data[i, 14] = jo["PositionYS"];
                data[i, 15] = jo["Data_LastChangeTime"];

            }

            IRow row;
            ICell cell;
            for (int i = 0; i < rowCount; i++)
            {
                row = sheet.CreateRow(i);//创建第i行
                for (int j = 0; j < columnCount; j++)
                {
                    cell = row.CreateCell(j);//创建第j列
                    object obj = data[i, j];
                    SetCellValue(cell, obj);
                }
            }

            try
            {
                FileStream fs = System.IO.File.OpenWrite(filePath);
                wb.Write(fs);//向打开的这个Excel文件中写入表单并保存。  
                fs.Close();
            }
            catch (Exception e)
            {

                logger.Error(e);
            }
            //TODO
            joResult["filepath"] = filePath2;
            return joResult.ToString();
        }
        //根据数据类型设置不同类型的cell
        public static void SetCellValue(ICell cell, object obj)
        {
            if (obj.GetType() == typeof(int))
            {
                cell.SetCellValue((int)obj);
            }
            else if (obj.GetType() == typeof(double))
            {
                cell.SetCellValue((double)obj);
            }
            else if (obj.GetType() == typeof(IRichTextString))
            {
                cell.SetCellValue((IRichTextString)obj);
            }
            else if (obj.GetType() == typeof(string))
            {
                cell.SetCellValue(obj.ToString());
            }
            else if (obj.GetType() == typeof(DateTime))
            {
                cell.SetCellValue((DateTime)obj);
            }
            else if (obj.GetType() == typeof(bool))
            {
                cell.SetCellValue((bool)obj);
            }
            else
            {
                cell.SetCellValue(obj.ToString());
            }
        }


         public string querycarstatus2(string CarNo, string startDateTime, string endDateTime,int istart, int ilen)
        {

            JObject joResult = new JObject();
            JArray messageList = new JArray();
            long totalCount = 0;
            CarStatusInfoDao2.GetCarStatus(CarNo, startDateTime, endDateTime,ilen, istart / ilen + 1, out totalCount, ref messageList);
            joResult["messages"] = messageList;
            joResult["total-records"] = totalCount;
            return joResult.ToString();
        }
    }

}
