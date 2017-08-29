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
    public class RoleInfoDao
    {
        public readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static IList<RoleInfo> GetAllRoles()
        {
    
            try
            {
                cardb2Entities1 cardb = new cardb2Entities1();
                IList<RoleInfo> roleinfos = cardb.RoleInfo.ToList<RoleInfo>();
                return roleinfos;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static void InsertRoleInfo(RoleInfo roleinfo)
        {
            try
            {
                cardb2Entities1 cardb = new cardb2Entities1();
                cardb.RoleInfo.Add(roleinfo);
                cardb.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public static void DeleteRoleInfo(int roleid)
        {
            try
            {
                cardb2Entities1 cardb = new cardb2Entities1();
                var role = cardb.RoleInfo.Find(roleid);
                cardb.RoleInfo.Remove(role);
                cardb.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public static void UpdateRoleInfo(RoleInfo roleinfo)
        {
            try
            {
                cardb2Entities1 cardb = new cardb2Entities1();
                var u = cardb.RoleInfo.Find(roleinfo.id);
                u.page = roleinfo.page;
                u.rolename = roleinfo.rolename;
                u.lasttime = DateTime.Now;
                cardb.SaveChanges();
            }
            catch (Exception ex)
            {

            }

        }

        public static void GetRoleList(string rolename, int pagesize, int pageNo, out long totalCount, ref JArray messagelist)
        {
            totalCount = 0;
            try
            {

                //计算ROWNUM
                int fromRowNum = (pageNo - 1) * pagesize + 1;
                int endRowNum = pagesize * pageNo;
                List<SqlParameter> sqlparlist = new List<SqlParameter>();
                StringBuilder whereCondition = new StringBuilder();

                if (!string.IsNullOrEmpty(rolename))
                {
                    whereCondition.Append(" and rolename ='" + rolename + "'");
                }

                StringBuilder sbSql = new StringBuilder();
                sbSql.Append(string.Format(CTE_SQL_ROLE, whereCondition));
                sbSql.Append(SQL_LIST_FROM_CTE);
                StringBuilder sbSql2 = new StringBuilder();
                sbSql2.Append(string.Format(CTE_SQL_ROLE, whereCondition));
                sbSql2.Append(SQL_COUNT);


                sqlparlist.Add(new SqlParameter("@from", fromRowNum));
                sqlparlist.Add(new SqlParameter("@end", endRowNum));

                SqlParameter[] sqlp = sqlparlist.ToArray();
                Dictionary<string, JObject> dic = new Dictionary<string, JObject>();

                using (cardb2Entities1 cardb = new cardb2Entities1())
                {
                    totalCount = cardb.Database.SqlQuery<int>(sbSql2.ToString()).FirstOrDefault();
                    var query = cardb.Database.SqlQuery<RoleInfo>(sbSql.ToString(), sqlp);
                    foreach (var item in query)
                    {
                        JObject joRow = new JObject();
                        joRow["id"] = item.id;
                        joRow["rolename"] = item.rolename;
                        joRow["page"] = item.page;
                        joRow["createtime"] = item.createtime;
                        joRow["lasttime"] = item.lasttime;

                        messagelist.Add(joRow);

                    }

                }

                logger.Info("GetRoleList Success");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

        }

        private const string CTE_SQL_ROLE = @"WITH CTE AS (select  ROW_NUMBER() OVER(ORDER BY id ASC ) as RowNumber , id,rolename,page,createtime,lasttime from RoleInfo
                                                    where 1=1 {0})";
        private const string SQL_LIST_FROM_CTE = "SELECT id,rolename,page,createtime,lasttime  from CTE where  RowNumber  BETWEEN @from AND @end ";
        private const string SQL_COUNT = @"SELECT COUNT(*) AS cnt FROM CTE ";
     
    }
}