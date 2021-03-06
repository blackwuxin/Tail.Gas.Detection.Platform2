﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using Tail.Gas.Detection.Platform.Models;

namespace Tail.Gas.Detection.Platform.Dao
{
    public class CarStatusInfoDao
    {
        public readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       
        public static IList<CarStatusInfo> GetAll()
        {
            using (cardbEntities cardb = new cardbEntities())
            {
                 IList<CarStatusInfo> carstatusinfos = cardb.CarStatusInfo.ToList<CarStatusInfo>();
                 return carstatusinfos;
           }
           
        }

        public static void InsertCarStatusInfo(CarStatusInfo carstatusinfo)
        {
            using (cardbEntities cardb = new cardbEntities())
            {
                cardb.CarStatusInfo.Add(carstatusinfo);
                cardb.SaveChanges();
            }
        }

        public static void GetNormalListByPage(string CarNo,string Category, string Belong, int pagesize, int pageNo, out long totalCount, ref JArray messagelist)
        {
            totalCount = 0;
            try
            {
               
                //计算ROWNUM
                int fromRowNum = (pageNo - 1) * pagesize + 1;
                int endRowNum = pagesize * pageNo;
                List<SqlParameter> sqlparlist = new List<SqlParameter>();
                StringBuilder whereCondition = new StringBuilder();
                whereCondition.Append(" and b.SystemStatus = 0 ");
                if (!string.IsNullOrEmpty(Category))
                {
                    whereCondition.Append(" and a.Category = '" + Category + "'");
                }
                if (!string.IsNullOrEmpty(Belong))
                {
                    whereCondition.Append(" and a.Belong ='" + Belong + "'");
                }
                if (!string.IsNullOrEmpty(CarNo))
                {
                    whereCondition.Append(" and a.no ='" + CarNo + "'");
                }
                StringBuilder sbSql = new StringBuilder();
				sbSql.Append(string.Format(CTE_SQL_LIST, whereCondition));
                sbSql.Append(SQL_LIST_FROM_CTE);
                StringBuilder sbSql2 = new StringBuilder();
                sbSql2.Append(string.Format(CTE_SQL_LIST, whereCondition));
                sbSql2.Append(SQL_COUNT);
                

                sqlparlist.Add(new SqlParameter("@from",fromRowNum));
                sqlparlist.Add(new SqlParameter("@end",endRowNum));

                SqlParameter[] sqlp = sqlparlist.ToArray(); 
                Dictionary<string, JObject> dic = new Dictionary<string, JObject>();

                using(cardbEntities cardb = new cardbEntities())
                {
                    totalCount = cardb.Database.SqlQuery<int>(sbSql2.ToString()).FirstOrDefault();
                    var query = cardb.Database.SqlQuery<CarStatus>(sbSql.ToString(), sqlp);
                    foreach (var item in query)
                    {
                        JObject joRow = new JObject();
                        joRow["CarNo"] = item.CarNo;
                        joRow["Color"] = item.Color;
                        joRow["Category"] = item.Category;
                        joRow["Belong"] = item.Belong;
                        joRow["TemperatureBefore"] = item.TemperatureBefore;
                        joRow["TemperatureAfter"] = (item.TemperatureAfter == null ? 0 : item.TemperatureAfter);
                        joRow["SensorNum"] = item.SensorNum;
                        joRow["SystemStatus"] = item.SystemStatus;
                        joRow["Data_LastChangeTime"] = String.Format("{0:yyyy-MM-dd HH:mm}", item.Data_LastChangeTime);
                        joRow["EngineSpeed"] = item.EngineSpeed;


                        //if ("豫AC8760,豫AR0038,豫AL1098".IndexOf(item.CarNo) != -1)
                        //{
                        //    if (dic.ContainsKey(item.CarNo))
                        //    {
                        //        dic.Remove(item.CarNo);
                        //    }
                                dic.Add(item.CarNo, joRow);
                         
                        //}
                      
                    }


                    foreach(var value in dic.Values){
                        messagelist.Add(value);
                    }
                }

                //totalCount = messagelist.Count();
                logger.Info("GetNormalListByPage Success");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            
        }

        public static void GetErrorListByPage(string CarNo, string Category, string Belong, int pagesize, int pageNo, out long totalCount, ref JArray messagelist)
        {

            totalCount = 0;
            try
            {

                //计算ROWNUM
                int fromRowNum = (pageNo - 1) * pagesize + 1;
                int endRowNum = pagesize * pageNo;
                List<SqlParameter> sqlparlist = new List<SqlParameter>();
                StringBuilder whereCondition = new StringBuilder();
                whereCondition.Append(" and b.SystemStatus != 0 ");
                if (!string.IsNullOrEmpty(Category))
                {
                    whereCondition.Append(" and a.Category = '" + Category + "'");
                }
                if (!string.IsNullOrEmpty(Belong))
                {
                    whereCondition.Append(" and a.Belong ='" + Belong + "'");
                }
                if (!string.IsNullOrEmpty(CarNo))
                {
                    whereCondition.Append(" and a.no ='" + CarNo + "'");
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

                 using (cardbEntities cardb = new cardbEntities())
                {
                    totalCount = cardb.Database.SqlQuery<int>(sbSql2.ToString()).FirstOrDefault();
                    var query = cardb.Database.SqlQuery<CarStatus>(sbSql.ToString(), sqlp);
                    foreach (var item in query)
                    {
                        JObject joRow = new JObject();
                        joRow["CarNo"] = item.CarNo;
                        joRow["Color"] = item.Color;
                        joRow["Category"] = item.Category;
                        joRow["Belong"] = item.Belong;
                        joRow["TemperatureBefore"] = item.TemperatureBefore;
                        joRow["TemperatureAfter"] = (item.TemperatureAfter == null ? 0 : item.TemperatureAfter);
                        joRow["SensorNum"] = item.SensorNum;
                        joRow["SystemStatus"] = item.SystemStatus;
                        joRow["Data_LastChangeTime"] = item.Data_LastChangeTime;
                        joRow["EngineSpeed"] = item.EngineSpeed;
                        
                        //  if ("豫AC8760,豫AR0038,豫AL1098".IndexOf(item.CarNo) != -1)
                        //{
                        //    if (dic.ContainsKey(item.CarNo))
                        //    {
                        //        dic.Remove(item.CarNo);
                        //    }
                                dic.Add(item.CarNo, joRow);
                         
                        //}
                      
                    }


                    foreach(var value in dic.Values){
                        messagelist.Add(value);
                    }
                
               }
                // totalCount = messagelist.Count();
                logger.Info("GetErrorListByPage Success");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

        }

        class ComparerClass : System.Collections.Generic.IEqualityComparer<JObject>
        {
            public bool Equals(JObject a, JObject b)
            {
                if (a == null && b == null)
                    return true;
                else if (a != null && b != null)
                {
                    if (a["CarNo"] != b["CarNo"])
                        return true;
                   
                    return false;
                }
                else return false;
            }
            public int GetHashCode(JObject ary)
            {
                return base.GetHashCode();
            }
        }
        public class CarStatus
        {
            public string CarNo{get;set;}
            public string Color { get; set; }
            public string Category { get; set; }
            public string Belong { get; set; }
            public decimal? TemperatureBefore { get; set; }
            public decimal? TemperatureAfter { get; set; }
            public decimal? SensorNum { get; set; }
            public int? SystemStatus { get; set; }
            public DateTime? Data_LastChangeTime { get; set; }
            public decimal? EngineSpeed { get; set; }
        }

//        private const string CTE_SQL_LIST = @"WITH CTE AS (select  ROW_NUMBER() OVER(ORDER BY a.no ASC ) as RowNumber, b.CarNo,a.Color,a.Category,a.Belong,b.TemperatureBefore,b.SensorNum,b.SystemStatus,b.Data_LastChangeTime from (select *,ROW_NUMBER() over(partition by carno order by Data_LastChangeTime desc) rn from CarStatusInfo) b ,carinfo a 
//                                              where b.rn<=1 and a.no = b.carno and a.no in('豫AC8760','豫AR0038','豫AL1098') {0})";
        private const string CTE_SQL_LIST = @"WITH CTE AS (select  ROW_NUMBER() OVER(ORDER BY a.no ASC ) as RowNumber, b.CarNo,a.Color,a.Category,a.Belong,b.TemperatureBefore,b.TemperatureAfter,b.SensorNum,b.SystemStatus,b.Data_LastChangeTime,b.EngineSpeed from (select *,ROW_NUMBER() over(partition by carno order by Data_LastChangeTime desc) rn from CarStatusInfo) b ,carinfo a 
                                              where b.rn<=1 and a.no = b.carno  {0})";
        //private const string SQL_LIST_FROM_CTE = "SELECT CarNo,Color,Category,Belong,TemperatureBefore,SensorNum,SystemStatus,Data_LastChangeTime from CTE where rownumber in	( select max(rownumber) as rownumber  from CTE group by carno) and RowNumber  BETWEEN @from AND @end ";
        private const string SQL_LIST_FROM_CTE = "SELECT CarNo,Color,Category,Belong,TemperatureBefore,TemperatureAfter,SensorNum,SystemStatus,Data_LastChangeTime,EngineSpeed from CTE where  RowNumber  BETWEEN @from AND @end ";
        private const string SQL_COUNT = @"SELECT COUNT(DISTINCT carno) AS cnt FROM CTE ";

    }
}