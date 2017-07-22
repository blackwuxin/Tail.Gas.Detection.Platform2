using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using Tail.Gas.Detection.Platform.Models;


namespace Tail.Gas.Detection.Platform.Dao
{
    public class CarInfoDao2
    {
        public readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public static IList<CarInfo> GetAlltest()
        {
            cardb2Entities1 cardb = new cardb2Entities1();
           
            IList<CarInfo> carinfos = cardb.CarInfo.ToList<CarInfo>();
            return carinfos;
            
        }

        public static void InsertCarInfo(CarInfo carinfo)
        {
            using (cardb2Entities1 cardb = new cardb2Entities1())
            {
                cardb.CarInfo.Add(carinfo);
                cardb.SaveChanges();
            }
        }

        public static CarInfo GetCardInfoByCarNo(string carno)
        {
            try
            {
                using (cardb2Entities1 cardb = new cardb2Entities1())
                {
                   // return cardb.CarInfo.LastOrDefault(x => x.NO == carno);
                    return cardb.CarInfo.OrderByDescending(x => x.ID).FirstOrDefault(x => x.NO == carno);
                }
                logger.Info("GetCardInfoByCarNo Success");
            }
            catch (Exception ex)
            {
                logger.Error("GetCardInfoByCarNo",ex);
                return null;
            }
           
        }

        public static void DeleteCarInfo()
        {
            using (cardb2Entities1 cardb = new cardb2Entities1())
            {
                try
                {
                    var count = cardb.Database.ExecuteSqlCommand(DELETESQL);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
               

            }
        }

        public static void GetCarListByPage(string CarNo, string Belong, string StartDateTime, string EndDateTime, int pagesize, int pageNo, out long totalCount, ref JArray messagelist)
        {

            totalCount = 0;
            try
            {

                //计算ROWNUM
                int fromRowNum = (pageNo - 1) * pagesize + 1;
                int endRowNum = pagesize * pageNo;
                List<SqlParameter> sqlparlist = new List<SqlParameter>();
                StringBuilder whereCondition = new StringBuilder();
                if (!string.IsNullOrEmpty(Belong))
                {
                    whereCondition.Append(" and Belong ='" + Belong + "'");
                }
                if (!string.IsNullOrEmpty(CarNo))
                {
                    whereCondition.Append(" and no ='" + CarNo + "'");
                }
                if (!string.IsNullOrEmpty(StartDateTime))
                {
                    whereCondition.Append("AND ModifiedTime >= '" + StartDateTime+"'");
                }
                if (!string.IsNullOrEmpty(EndDateTime))
                {
                    whereCondition.Append(" AND ModifiedTime <=  '" + EndDateTime + "'");
                }
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append(string.Format(CTE_SQL_LIST, whereCondition));
                sbSql.Append(SQL_LIST_FROM_CTE);
                StringBuilder sbSql2 = new StringBuilder();
                sbSql2.Append(string.Format(CTE_SQL_LIST, whereCondition));
                sbSql2.Append(SQL_COUNT);


                sqlparlist.Add(new SqlParameter("@from", fromRowNum));
                sqlparlist.Add(new SqlParameter("@end", endRowNum));

                SqlParameter[] sqlp = sqlparlist.ToArray();
                Dictionary<string, JObject> dic = new Dictionary<string, JObject>();

                using (cardb2Entities1 cardb = new cardb2Entities1())
                {
                    totalCount = cardb.Database.SqlQuery<int>(sbSql2.ToString()).FirstOrDefault();
                    var query = cardb.Database.SqlQuery<CarInfo>(sbSql.ToString(), sqlp);

                    foreach (var item in query)
                    {
                        JObject joRow = new JObject();
                        joRow["NO"] = item.NO;
                        joRow["Category"] = item.Category;
                        joRow["Belong"] = item.Belong;
                        //joRow["EngineNo"] = item.EngineNo;
                        joRow["OriginalEmissionValues"] = item.OriginalEmissionValues;
                        joRow["ProductModel"] = item.ProductModel;
                        joRow["ModifiedTime"] = item.ModifiedTime;
                        joRow["UserInfo"] = item.UserInfo;
                        joRow["IndividualCompany"] = item.IndividualCompany;
                        //joRow["Data_LastChangeTime"] = item.Data_LastChangeTime;
                        //joRow["Data_CreateTime"] = item.Data_CreateTime;
                        joRow["Color"] = item.Color;
                        joRow["ModifiedCompany"] = item.ModifiedCompany;
                        //joRow["CarType"] = item.CarType;
                        joRow["Telphone"] = item.Telphone;
                        
                        dic.Add(item.NO, joRow);
                    }


                    foreach (var value in dic.Values)
                    {
                        messagelist.Add(value);
                    }

                }
                logger.Info("GetCarListByPage Success");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

        }
        private const string CTE_SQL_LIST = @"WITH CTE AS (select  ROW_NUMBER() OVER(ORDER BY ModifiedTime desc ) as RowNumber,ID,NO,Category ,Belong,EngineNo ,OriginalEmissionValues,ProductModel ,ModifiedTime,UserInfo,IndividualCompany,Telphone ,Data_CreateTime ,Data_LastChangeTime,Color ,ModifiedCompany,CarType from carinfo where 1=1 {0})";
        private const string SQL_LIST_FROM_CTE = "SELECT ID,NO,Category ,Belong,EngineNo ,OriginalEmissionValues,ProductModel ,ModifiedTime,UserInfo,IndividualCompany,Telphone ,Data_CreateTime ,Data_LastChangeTime,Color ,ModifiedCompany,CarType from CTE where  RowNumber  BETWEEN @from AND @end ";
        private const string SQL_COUNT = @"SELECT COUNT(DISTINCT no) AS cnt FROM CTE ";
        const string DELETESQL = "  delete from carinfo where no in  (select no from CarInfo group by no having count(no)>1) and id not in (select max(id) from CarInfo group by no having count(no) >1)";
    }
}