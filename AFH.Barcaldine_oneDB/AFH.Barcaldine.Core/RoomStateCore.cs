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

            DateTime startDate = new DateTime(model.Year, model.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            string[] chooseList = model.ChooseDate.Split(',');

            for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
            {
                str = new StringBuilder();
                str.AppendLine("Insert into RoomStatus");
                str.AppendLine(" (StatusDate, RoomType, Available,CreateUser,CreateTime,UpdateUser,UpdateTime)");
                str.AppendLine(" Values (@StatusDate, @RoomType, @Available, @CreateUser, getdate(), @UpdateUser, getdate())");

                SqlParameter[] paras = new SqlParameter[5];
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
                
                paras[3] = new SqlParameter("@CreateUser", Common.VariableConvert.ConvertStringToDBValue(model.CreateUser));
                paras[4] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(model.UpdateUser));

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

        /// <summary>
        /// online
        /// </summary>
        /// <param name="model"></param>
        public AccomodationOrderModel GetAvailableRoom(AccomodationOrderModel model)
        {

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT m.RoomType, MIN(m.Availablecount) as Availablecount");
            sql.AppendLine(" from (");
            sql.AppendLine("        select t.StatusDate, t.RoomType, COUNT(t.Availablecount) as Availablecount");
            sql.AppendLine("        from (");
            sql.AppendLine("                select StatusDate, RoomType = case RoomType when 0 then 0 when 1 then 1 when 2 then 1 end, COUNT(1) as Availablecount");
            sql.AppendLine("                from RoomStatus");
            sql.AppendLine("                where StatusDate >= '2015-01-01' and StatusDate <= '2015-01-02' and Available=0");
            sql.AppendLine("                group by StatusDate, RoomType");
            sql.AppendLine("        ) as t");
            sql.AppendLine("        group by t.StatusDate, t.RoomType");
            sql.AppendLine("    ) m");
            sql.AppendLine(" where m.Availablecount>0 ");
            sql.AppendLine(" group by m.RoomType");



            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("@CheckinDate", model.CheckInDate);
            paras[1] = new SqlParameter("@CheckoutDate", model.CheckOutDate);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            model.AccomodationOrderList = new List<AccomodationOrderListModel>();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow rw in ds.Tables[0].Rows)
                {
                    AccomodationOrderListModel order = new AccomodationOrderListModel();
                    order.RoomType = Convert.ToInt32(rw["RoomType"]);
                    order.AvailableRoomNumber = Convert.ToInt32(rw["Availablecount"]);
                    model.AccomodationOrderList.Add(order);
                }
            }
            return model;

        }


    }
}
