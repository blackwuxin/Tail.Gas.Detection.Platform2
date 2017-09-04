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
    public class UserInfoDao
    {
        public readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void InsertUserInfo(UserInfo userinfo)
        {

                cardb2Entities1 cardb = new cardb2Entities1();
                var user = cardb.UserInfo.Where(u => u.username == userinfo.username);

                if (user.Count() > 0)
                {
                    throw new Exception("-2");
                }
                    cardb.UserInfo.Add(userinfo);
                    cardb.SaveChanges();


        }
        public static void DeleteUserInfo(int userid)
        {
            try
            {
                cardb2Entities1 cardb = new cardb2Entities1();
                var user = cardb.UserInfo.Find(userid);
                cardb.UserInfo.Remove(user);
                cardb.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public static string CheckUserInfo(string username,string password,string usertype)
        {
            try
            {
                cardb2Entities1 cardb = new cardb2Entities1();
                var user = cardb.UserInfo.Where( u => (u.username == username && u.password == password));
                var role = cardb.UserRoleInfo.Where(u => (u.rolename == usertype && u.username == username));
                if(user.Count()>0 && role.Count()>0){
                    return role.First().page;
                }
                
            }
            catch (Exception ex)
            {

            }
            return "error";
        }
        public static void UpdateUserInfo(UserInfo userinfo)
        {
            try
            {
                cardb2Entities1 cardb = new cardb2Entities1();
                var u = cardb.UserInfo.Find(userinfo.id);
                u.username = userinfo.username;
                u.password = userinfo.password;
                u.lasttime = DateTime.Now;
                cardb.SaveChanges();
            }
            catch (Exception ex)
            {

            }

        }

        public static void GetUserList(string username, int pagesize, int pageNo, out long totalCount, ref JArray messagelist)
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
                    whereCondition.Append(" and username ='" + username + "'");
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
                    var query = cardb.Database.SqlQuery<UserInfo>(sbSql.ToString(), sqlp);
                    foreach (var item in query)
                    {
                        JObject joRow = new JObject();
                        joRow["id"] = item.id;
                        joRow["username"] = item.username;
                        joRow["password"] = item.password;
                        joRow["createtime"] = item.createtime;
                        joRow["lasttime"] = item.lasttime;

                        messagelist.Add(joRow);

                    }

                }

                logger.Info("GetUserList Success");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

        }

        private const string CTE_SQL_ROLE = @"WITH CTE AS (select  ROW_NUMBER() OVER(ORDER BY id ASC ) as RowNumber , id,username,password,createtime,lasttime from UserInfo
                                                    where 1=1 {0})";
        private const string SQL_LIST_FROM_CTE = "SELECT id,username,password,createtime,lasttime  from CTE where  RowNumber  BETWEEN @from AND @end ";
        private const string SQL_COUNT = @"SELECT COUNT(*) AS cnt FROM CTE ";
     
    }
}