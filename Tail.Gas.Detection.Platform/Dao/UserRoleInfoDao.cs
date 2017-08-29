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

        public static IList<RoleInfo> GetAllRoles()
        {
            cardb2Entities1 cardb = new cardb2Entities1();
            IList<RoleInfo> roleinfos = cardb.RoleInfo.ToList<RoleInfo>();
            return roleinfos;

        }
        public static IList<UserInfo> GetAllUsers()
        {
            cardb2Entities1 cardb = new cardb2Entities1();
            IList<UserInfo> userinfos = cardb.UserInfo.ToList<UserInfo>();
            return userinfos;
        }

        public static void InsertUserfo(UserInfo userinfo)
        {
            using (cardb2Entities1 cardb = new cardb2Entities1())
            {
                cardb.UserInfo.Add(userinfo);
                cardb.SaveChanges();
            }
        }

        public static void InsertRoleInfo(RoleInfo roleinfo)
        {
            using (cardb2Entities1 cardb = new cardb2Entities1())
            {
                cardb.RoleInfo.Add(roleinfo);
                cardb.SaveChanges();
            }
        }
        public static void InsertUserRoleInfo(UserRoleInfo userroleinfo)
        {
            using (cardb2Entities1 cardb = new cardb2Entities1())
            {
                cardb.UserRoleInfo.Add(userroleinfo);
                cardb.SaveChanges();
            }
        }
        public void DeleteUserRole(UserRoleInfo userRole)
        {
            try
            {
                cardb2Entities1 cardb = new cardb2Entities1();
                Object result = cardb.UserRoleInfo.Remove(userRole);
                cardb.SaveChanges();
            }
            catch (Exception ex)
            {
               
            }
        }
        public void UpdateUserRole(UserRoleInfo userroleinfo)
        {
            using (cardb2Entities1 cardb = new cardb2Entities1())
            {
                //var u = cardb.UserRoleInfo.Find(userroleinfo.ID);
                //u.roletype = userroleinfo.roletype;
                //u.userid = userroleinfo.userid;
                cardb.SaveChanges();
            }
        }
        public static void InsertPageInfo(PageInfo pageinfo)
        {
            using (cardb2Entities1 cardb = new cardb2Entities1())
            {
                cardb.PageInfo.Add(pageinfo);
                cardb.SaveChanges();
            }
        }

        public static void GetAllUserRoleDetails(string username, string rolename, int pagesize, int pageNo, out long totalCount, ref JArray messagelist)
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
                    var query = cardb.Database.SqlQuery<UserRoleDetail>(sbSql.ToString(), sqlp);
                    foreach (var item in query)
                    {
                        JObject joRow = new JObject();
                        joRow["username"] = item.username;
                        joRow["rolename"] = item.rolename;
                        joRow["page"] = item.page;


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

        public class UserRoleDetail
        {
            public string username { get; set; }
            public string rolename { get; set; }
            public string page { get; set; }

        }

        private const string CTE_SQL_USERROLE = @"WITH CTE AS (select  ROW_NUMBER() OVER(ORDER BY a.username ASC ) as RowNumber , username,rolename,page from UserInfo a ,RoleInfo b 
                                                    where a.usertype = b.roletype {0}))";
        private const string SQL_LIST_FROM_CTE = "SELECT username,rolename,page  from CTE where  RowNumber  BETWEEN @from AND @end ";
        private const string SQL_COUNT = @"SELECT COUNT(*) AS cnt FROM CTE ";
    }
}