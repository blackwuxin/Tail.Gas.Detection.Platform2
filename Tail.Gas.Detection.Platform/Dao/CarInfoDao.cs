﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tail.Gas.Detection.Platform.Models;

namespace Tail.Gas.Detection.Platform.Dao
{
    public class CarInfoDao
    {
        public readonly static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static IList<CarInfo> GetAlltest()
        {
            cardbEntities cardb = new cardbEntities();
            IList<CarInfo> carinfos = cardb.CarInfo.ToList<CarInfo>();
            return carinfos;
            
        }

        public static void InsertCarInfo(CarInfo carinfo)
        {
            using (cardbEntities cardb = new cardbEntities())
            {
                cardb.CarInfo.Add(carinfo);
                cardb.SaveChanges();
            }
        }

        public static CarInfo GetCardInfoByCarNo(string carno)
        {
            try
            {
                using (cardbEntities cardb = new cardbEntities())
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
            using (cardbEntities cardb = new cardbEntities())
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

        const string DELETESQL = "  delete from carinfo where no in  (select no from CarInfo group by no having count(no)>1) and id not in (select max(id) from CarInfo group by no having count(no) >1)";
    }
}