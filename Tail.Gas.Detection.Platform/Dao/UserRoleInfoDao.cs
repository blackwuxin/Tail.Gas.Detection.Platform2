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
    public class UserRoleInfoDao
    {
        public readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);




        public static void InsertUserRoleInfo(UserRoleInfo userroleinfo)
        {
            using (cardb2Entities1 cardb = new cardb2Entities1())
            {
                var user = cardb.UserRoleInfo.Select(u => u.username == userroleinfo.username).FirstOrDefault();
                if (!user)
                {
                    var role = cardb.RoleInfo.Where(r => r.rolename == userroleinfo.rolename).FirstOrDefault();
                    userroleinfo.page = role.page;
                    cardb.UserRoleInfo.Add(userroleinfo);
                    cardb.SaveChanges();
                }
              
            }
        }
        public static void DeleteUserRole(int userroleid)
        {
            try
            {

                cardb2Entities1 cardb = new cardb2Entities1();
                var user = cardb.UserRoleInfo.Find(userroleid);
                cardb.UserRoleInfo.Remove(user);
                cardb.SaveChanges();
            }
            catch (Exception ex)
            {
               
            }
        }
        public static void UpdateUserRole(UserRoleInfo userroleinfo)
        {
            using (cardb2Entities1 cardb = new cardb2Entities1())
            {
                var u = cardb.UserRoleInfo.Find(userroleinfo.id);
                u.username = userroleinfo.username;
                u.rolename = userroleinfo.rolename;
                var role = cardb.RoleInfo.Where(r => r.rolename == userroleinfo.rolename).FirstOrDefault();
                u.page = role.page;
                u.lasttime = DateTime.Now;
                cardb.SaveChanges();
            }
        }
      
        public static void GetUserRoleList(string username, string rolename, int pagesize, int pageNo, out long totalCount, ref JArray messagelist)
        {
            totalCount = 0;
            try
            {

                //计算ROWNUM
                int fromRowNum = (pageNo - 1) * pagesize + 1;
                int endRowNum = pagesize * pageNo;
                List<SqlParameter> sqlparlist = new List<SqlParameter>();
                StringBuilder whereCondition = new StringBuilder();
                if (!string.IsNullOrEmpty(username))
                {
                    whereCondition.Append(" and username = '" + username + "'");
                }
                if (!string.IsNullOrEmpty(rolename))
                {
                    whereCondition.Append(" and rolename ='" + rolename + "'");
                }

                StringBuilder sbSql = new StringBuilder();
                sbSql.Append(string.Format(CTE_SQL_USERROLE, whereCondition));
                sbSql.Append(SQL_LIST_FROM_CTE);
                StringBuilder sbSql2 = new StringBuilder();
                sbSql2.Append(string.Format(CTE_SQL_USERROLE, whereCondition));
                sbSql2.Append(SQL_COUNT);


                sqlparlist.Add(new SqlParameter("@from", fromRowNum));
                sqlparlist.Add(new SqlParameter("@end", endRowNum));

                SqlParameter[] sqlp = sqlparlist.ToArray();
                Dictionary<string, JObject> dic = new Dictionary<string, JObject>();

                using (cardb2Entities1 cardb = new cardb2Entities1())
                {
                    totalCount = cardb.Database.SqlQuery<int>(sbSql2.ToString()).FirstOrDefault();
                    var query = cardb.Database.SqlQuery<UserRoleInfo>(sbSql.ToString(), sqlp);
                    foreach (var item in query)
                    {
                        JObject joRow = new JObject();
                        joRow["id"] = item.id;
                        joRow["username"] = item.username;
                        joRow["rolename"] = item.rolename;
                        joRow["page"] = item.page;
                        joRow["createtime"] = item.createtime;
                        joRow["lasttime"] = item.lasttime;
                        messagelist.Add(joRow);

                    }

                }

                logger.Info("GetAllUserRoleDetails Success");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

        }

     

        private const string CTE_SQL_USERROLE = @"WITH CTE AS (select  ROW_NUMBER() OVER(ORDER BY id ASC ) as RowNumber , id,username,rolename,page,createtime,lasttime from UserRoleInfo 
                                                    where 1=1 {0})";
        private const string SQL_LIST_FROM_CTE = "SELECT  id,username,rolename,page,createtime,lasttime  from CTE where  RowNumber  BETWEEN @from AND @end ";
        private const string SQL_COUNT = @"SELECT COUNT(*) AS cnt FROM CTE ";
    }
}