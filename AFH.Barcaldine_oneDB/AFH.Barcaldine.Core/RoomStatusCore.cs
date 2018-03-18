using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using AFH.Barcaldine.Models;
using AFH.Barcaldine.Common;
using AFH.Common.DataBaseAccess;
using AFH.Common.Serializer;

namespace AFH.Barcaldine.Core
{
    public class RoomStatusCore
    {
        /// <summary>
        /// offline
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RoomStatusModel GetSearchData(RoomStatusModel model)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" RoomStatusID, StatusDate, RoomType, Available");
            sql.AppendLine(" FROM RoomStatus WITH(NOLOCK)");
            sql.AppendLine(" WHERE 1=1");
            sql.AppendLine(" and StatusDate >= @StartDate");
            sql.AppendLine(" and StatusDate <= @EndDate");
            sql.AppendLine(" and RoomType = @RoomType");
            sql.AppendLine(" order by StatusDate asc");


            SqlParameter[] paras = new SqlParameter[3];
            DateTime dtStart = new DateTime(model.Year, model.Month, 1);
            paras[0] = new SqlParameter("@StartDate", dtStart);
            DateTime dtEnd = dtStart.AddMonths(1).AddDays(-1);
            paras[1] = new SqlParameter("@EndDate", dtEnd);
            paras[2] = new SqlParameter("@RoomType", model.RoomType);


            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                //roomStatusModel.RoomStatusUpdate = DataMappingHelper<RoomStatusUpdateModel>.DataTableToList(ds.Tables[0]);
                model.OpertationStatus = GlobalVariable.UpdateStatus.Update;
                DataTable dt = ds.Tables[0];
                model.ChooseDate = string.Empty;
                foreach (DataRow dw in dt.Rows)
                {
                    if (Convert.ToBoolean(dw["Available"]))
                    {
                        model.ChooseDate += Convert.ToDateTime(dw["StatusDate"].ToString()).Day.ToString() + ",";
                    }
                }

            }
            else
            {
                model.OpertationStatus = GlobalVariable.UpdateStatus.Add;
            }
            return model;

        }

        /// <summary>
        /// offline
        /// </summary>
        /// <param name="model"></param>
        public void InsertData(RoomStatusModel model)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            StringBuilder str;

            DateTime startDate = new DateTime(model.Year, 1, 1);
            DateTime endDate = startDate.AddYears(1).AddDays(-1);

            for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
            {
                str = new StringBuilder();
                str.AppendLine("Insert into RoomStatus");
                str.AppendLine(" (StatusDate, RoomType, Available,CreateUser,CreateTime,UpdateUser,UpdateTime)");
                str.AppendLine(" Values (@StatusDate, @RoomType, 0, 'system', getdate(), 'system', getdate())");

                SqlParameter[] paras = new SqlParameter[2];
                paras[0] = new SqlParameter("@StatusDate", dt);
                paras[1] = new SqlParameter("@RoomType", model.RoomType);

                sqls.Add(str.ToString());
                cmdParms.Add(paras);

            }

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);
        }

        /// <summary>
        /// offline
        /// </summary>
        /// <param name="model"></param>
        public void UpdateData(RoomStatusModel model)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();
            string[] chooseList = model.ChooseDate.Split(',');

            StringBuilder str;

            DateTime startDate = new DateTime(model.Year, model.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
            {
                str = new StringBuilder();
                str.AppendLine("update RoomStatus set");
                str.AppendLine(" Available=@Available");
                str.AppendLine(" ,UpdateUser=@UpdateUser");
                str.AppendLine(" ,UpdateTime=getdate()");
                str.AppendLine(" where StatusDate=@StatusDate and RoomType=@RoomType");

                SqlParameter[] paras = new SqlParameter[4];
                paras[0] = new SqlParameter("@StatusDate", dt);
                paras[1] = new SqlParameter("@RoomType", model.RoomType);
                string[] isexist = Array.FindAll(chooseList, m => (m == dt.Day.ToString()));
                if (isexist.Length>0)
                {
                    paras[2] = new SqlParameter("@Available", true);
                }
                else
                {
                    paras[2] = new SqlParameter("@Available", false);
                }

                paras[3] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(model.UpdateUser));

                sqls.Add(str.ToString());
                cmdParms.Add(paras);

            }

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);
        }

        #region online: get available room 

        public AccomodationOrderModel GetAvailableRoom(AccomodationOrderModel order)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("select StatusDate, RoomType, Available ");
            sql.AppendLine(" from RoomStatus");
            sql.AppendLine(" where StatusDate >= @CheckinDate and StatusDate < @CheckoutDate");

            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("@CheckinDate", order.CheckInDate);
            paras[1] = new SqlParameter("@CheckoutDate", order.CheckOutDate);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            order.AccomodationOrderList = new List<AccomodationOrderListModel>();
            if (ds != null && ds.Tables.Count > 0)
            {

                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    order = this.GetKingAvailableRoom(Convert.ToDateTime(order.CheckInDate), Convert.ToDateTime(order.CheckOutDate), dt, order);//

                    order = this.GetQueenAvailableRoom(Convert.ToDateTime(order.CheckInDate), Convert.ToDateTime(order.CheckOutDate), dt, order);

                }
                else
                {
                    AccomodationOrderListModel orderListKing = new AccomodationOrderListModel();
                    orderListKing.RoomType = GlobalVariable.RoomType.King;
                    orderListKing.AvailableRoomNumber = 1;
                    order.AccomodationOrderList.Add(orderListKing);

                    AccomodationOrderListModel orderListQueen = new AccomodationOrderListModel();
                    orderListQueen.RoomType = GlobalVariable.RoomType.Queen;
                    orderListQueen.AvailableRoomNumber = 2;
                    order.AccomodationOrderList.Add(orderListQueen);

                }
            }

            return order;

        }

        private AccomodationOrderModel GetKingAvailableRoom(DateTime checkin, DateTime checkout,
            DataTable roomStatus, AccomodationOrderModel order)
        {
            TimeSpan ts = checkout - checkin;
            int staydays = ts.Days;
           
            var result = from myRom in roomStatus.AsEnumerable()
                         where myRom.Field<int>("RoomType") == 0 && myRom.Field<bool>("Available") == true
                         group myRom by myRom.Field<System.DateTime>("StatusDate") into g
                         select new { aa = g.Count(), g.Key  };
            
            if (result.Count() >= staydays)
            {
              //  List<AvailableRoom> ars = result.ToList();
                //var a = result.ToList();
                //var min = result.ToList();
                List<int> KingRoomCount = new List<int>();
                foreach (var item in result)
                {
                    KingRoomCount.Add(item.aa);
                }
                
                AccomodationOrderListModel orderListKing = new AccomodationOrderListModel();
                orderListKing.RoomType = GlobalVariable.RoomType.King;
                orderListKing.AvailableRoomNumber = KingRoomCount.Min();
                order.AccomodationOrderList.Add(orderListKing);
            }

            return order;
        }

        private AccomodationOrderModel GetQueenAvailableRoom(DateTime checkin, DateTime checkout,
            DataTable roomStatus, AccomodationOrderModel order)
        {
            int minRoomCount;
            List<int> RoomCount1 = new List<int>();
            int[] RoomCount = new int[5];
            int i = 0;
            for (DateTime dt = checkin; dt < checkout; dt = dt.AddDays(1))
            {
                var result = from myRom in roomStatus.AsEnumerable()
                             where (myRom.Field<int>("RoomType") == GlobalVariable.RoomType.Queen) //|| myRom.Field<int>("RoomType") == GlobalVariable.RoomType.Princess)
                                && myRom.Field<bool>("Available") == true && myRom.Field<DateTime>("StatusDate") == dt
                             select myRom;

                if (result.Count() == 0)
                {
                    //there is one day without any rooms
                    return order;
                }
                else 
                {
                    //one day just left one room
                    RoomCount1.Add(result.Count());
                   // RoomCount[i] = result.Count();
                  //  i++;
                }
            }

            var min = RoomCount1.Min();
            minRoomCount = min;
            AccomodationOrderListModel orderQueen = new AccomodationOrderListModel();
            orderQueen.RoomType = GlobalVariable.RoomType.Queen;
            orderQueen.AvailableRoomNumber = minRoomCount;
            order.AccomodationOrderList.Add(orderQueen);

            return order;
        }

        #endregion

        #region update room status for online

        public AccomodationOrderModel UpdateRoomStatus(AccomodationOrderModel order)
        {

            StringBuilder select = new StringBuilder();
            select.AppendLine("SELECT ");
            select.AppendLine(" RoomStatusID, StatusDate, RoomType, Available");
            select.AppendLine(" FROM RoomStatus WITH(NOLOCK)");
            select.AppendLine(" WHERE 1=1");
            select.AppendLine(" and StatusDate >= @CheckinDate");
            select.AppendLine(" and StatusDate < @CheckoutDate");
            select.AppendLine(" order by StatusDate asc");

            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("@CheckinDate", Convert.ToDateTime(order.CheckInDate));
            paras[1] = new SqlParameter("@CheckoutDate", Convert.ToDateTime(order.CheckOutDate));

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(select.ToString(), paras);


            List<string> sqlUpdates = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            foreach (AccomodationOrderListModel orderList in order.AccomodationOrderList)
            {
                if (orderList.OrderRoomNumber > 0)
                {
                    for (DateTime dt = Convert.ToDateTime(order.CheckInDate); dt < Convert.ToDateTime(order.CheckOutDate); dt = dt.AddDays(1))
                    {
                        string sql = string.Empty;
                        SqlParameter[] param = new SqlParameter[2];
                        if (orderList.RoomType == GlobalVariable.RoomType.King)
                        {
                            this.GetUpdateSql(dt, GlobalVariable.RoomType.King, ref sql, ref param);
                            sqlUpdates.Add(sql);
                            cmdParms.Add(param);
                        }
                        else if (orderList.RoomType == GlobalVariable.RoomType.Queen)
                        {
                            this.GetUpdateRoomStatusForQueen(orderList.RoomType, dt, ds, orderList.OrderRoomNumber, ref sqlUpdates, ref cmdParms);
                        }

                    }
                }
            }

            mySqlAccess.ExecuteNonQuerys(sqlUpdates, cmdParms);

            return order;
        }

        private void GetUpdateRoomStatusForQueen(int roomType, DateTime stayDate, DataSet currentRoomStatus, int orderNum,
            ref List<string> sqlUpdates, ref List<SqlParameter[]> cmdParms)
        {
            string sql = string.Empty;
            SqlParameter[] param = new SqlParameter[2];

            if (currentRoomStatus != null && currentRoomStatus.Tables.Count > 0 && currentRoomStatus.Tables[0].Rows.Count > 0)
            {
                if (orderNum == 2)
                {
                    this.GetUpdateSql(stayDate, GlobalVariable.RoomType.Queen, ref sql, ref param);
                    sqlUpdates.Add(sql);
                    cmdParms.Add(param);

                    sql = string.Empty;
                    param = new SqlParameter[2];
                    this.GetUpdateSql(stayDate, GlobalVariable.RoomType.Princess, ref sql, ref param);
                    sqlUpdates.Add(sql);
                    cmdParms.Add(param);

                }
                else if (orderNum == 1)
                {
                    var result = from myRom in currentRoomStatus.Tables[0].AsEnumerable()
                                 where myRom.Field<int>("RoomType") == GlobalVariable.RoomType.Queen
                                    && myRom.Field<DateTime>("StatusDate") == stayDate
                                    && myRom.Field<bool>("Available") == false
                                 select myRom;
                    if (result.Count() == 1)
                    {
                        this.GetUpdateSql(stayDate, GlobalVariable.RoomType.Queen, ref sql, ref param);
                    }
                    else if (result.Count() == 0)
                    {
                        this.GetUpdateSql(stayDate, GlobalVariable.RoomType.Princess, ref sql, ref param);
                    }
                    sqlUpdates.Add(sql);
                    cmdParms.Add(param);
                }
            }

        }

        private void GetUpdateSql(DateTime stayDate, int roomType, ref string sql, ref SqlParameter[] param)
        {
            sql = "Update RoomStatus set Available = 1";
            sql += " ,UpdateUser = 'system', UpdateTime = getdate()";
            sql += " where StatusDate=@StatusDate and RoomType = @RoomType";

            param[0] = new SqlParameter("@StatusDate", stayDate);
            param[1] = new SqlParameter("@RoomType", roomType);
        }


        #endregion
    }
}
