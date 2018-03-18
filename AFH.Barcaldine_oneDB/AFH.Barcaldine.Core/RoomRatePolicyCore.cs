using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

using AFH.Barcaldine.Common;
using AFH.Common.DataBaseAccess;
using AFH.Common.Serializer;
using AFH.Barcaldine.Models;

namespace AFH.Barcaldine.Core
{
    public class RoomRatePolicyCore
    {
        public List<RatePolicyResultModel> GetSearchData(RatePolicySearchModel search)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" RoomPolicyID, StartDate, EndDate, RoomType,");
            sql.AppendLine(" RoomTypeName = case RoomType when 0 then 'King' when 1 then 'Queen' end, ");
            sql.AppendLine(" IsDelete");
            sql.AppendLine(" FROM RoomPolicy WITH(NOLOCK)");
            sql.AppendLine(" WHERE 1=1");

            if (search.StartDateFrom != null)
            {
                sql.AppendLine(" and StartDate >= @StartDateFrom");
            }
            if (search.StartDateTo != null)
            {
                sql.AppendLine(" and StartDate <= @StartDateTo");
            }

            if (search.EndDateFrom != null)
            {
                sql.AppendLine(" and EndDate >= @EndDateFrom");
            }
            if (search.EndDateTo != null)
            {
                sql.AppendLine(" and EndDate <= @EndDateTo");
            }

            if (search.RoomType != 9)
            {
                sql.AppendLine(" and RoomType = @RoomType");    
            }
            if (search.IsDelete != "9")
            {
                sql.AppendLine(" and IsDelete = @IsDelete");
            }


            SqlParameter[] paras = new SqlParameter[3];
            if (search.StartDateFrom != null)
            {
                paras[0] = new SqlParameter("@StartDateFrom", search.StartDateFrom);
            }
            if (search.StartDateTo != null)
            {
                paras[1] = new SqlParameter("@StartDateTo", search.StartDateFrom);
            }

            if (search.StartDateFrom != null)
            {
                paras[0] = new SqlParameter("@EndDateFrom", search.EndDateFrom);
            }
            if (search.StartDateTo != null)
            {
                paras[1] = new SqlParameter("@EndDateTo", search.EndDateTo);
            }

            if (search.RoomType != 9)
            {
                paras[1] = new SqlParameter("@RoomType", search.RoomType);
            }
            if (search.IsDelete != "9")
            {
                paras[2] = new SqlParameter("@IsDelete", search.IsDelete);
            }

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            List<RatePolicyResultModel> ratePolicy = new List<RatePolicyResultModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                ratePolicy = DataMappingHelper<RatePolicyResultModel>.DataTableToList(ds.Tables[0]);

            }

            return ratePolicy;
        }

        public void InsertData(RatePolicyDetailModel model)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            StringBuilder str = new StringBuilder();
            str.AppendLine("Insert into RoomPolicy");
            str.AppendLine(" (StartDate, EndDate,RoomType,WeekDays,BasicRate,DiscountRate,IsDelete,CreateUser,CreateTime,UpdateUser,UpdateTime) ");
            str.AppendLine(" Values (@StartDate, @EndDate, @RoomType, @WeekDays, @BasicRate, @DiscountRate, @IsDelete,@CreateUser, getdate(), @UpdateUser,getdate());");

            SqlParameter[] paras = new SqlParameter[9];
            paras[0] = new SqlParameter("@StartDate", model.StartDate);
            paras[1] = new SqlParameter("@EndDate", model.EndDate);
            paras[2] = new SqlParameter("@RoomType", model.RoomType);
            paras[3] = new SqlParameter("@WeekDays", Common.VariableConvert.ConvertStringToDBValue(model.WeekDays));
            paras[4] = new SqlParameter("@BasicRate", model.BasicRate);
            paras[5] = new SqlParameter("@DiscountRate", Common.VariableConvert.ConvertStringToDBValue(model.DiscountRate));
            paras[6] = new SqlParameter("@IsDelete", Common.VariableConvert.BitConverter(model.IsDelete));
            paras[7] = new SqlParameter("@CreateUser", Common.VariableConvert.ConvertStringToDBValue(model.CreateUser));
            paras[8] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(model.UpdateUser));

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);
        }

        public void UpdateData(RatePolicyDetailModel model)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            StringBuilder str = new StringBuilder();
            str.AppendLine("update RoomPolicy set");
            str.AppendLine(" StartDate=@StartDate");
            str.AppendLine(" ,EndDate=@EndDate");
            str.AppendLine(" ,RoomType=@RoomType");
            str.AppendLine(" ,WeekDays=@WeekDays");
            str.AppendLine(" ,BasicRate=@BasicRate");
            str.AppendLine(" ,DiscountRate=@DiscountRate");
            str.AppendLine(" ,IsDelete=@IsDelete");
            str.AppendLine(" ,UpdateUser=@UpdateUser");
            str.AppendLine(" ,UpdateTime=getdate()");
            str.AppendLine(" where RoomPolicyID=@RoomPolicyID");

            SqlParameter[] paras = new SqlParameter[9];
            paras[0] = new SqlParameter("@StartDate", model.StartDate);
            paras[1] = new SqlParameter("@EndDate", model.EndDate);
            paras[2] = new SqlParameter("@RoomType", model.RoomType);
            paras[3] = new SqlParameter("@WeekDays", Common.VariableConvert.ConvertStringToDBValue(model.WeekDays));
            paras[4] = new SqlParameter("@BasicRate", model.BasicRate);
            paras[5] = new SqlParameter("@DiscountRate", Common.VariableConvert.ConvertStringToDBValue(model.DiscountRate));
            paras[6] = new SqlParameter("@IsDelete", Common.VariableConvert.BitConverter(model.IsDelete));
            paras[7] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(model.UpdateUser));
            paras[8] = new SqlParameter("@RoomPolicyID", model.RoomPolicyID);

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);
        }

        public bool IsExistRoomRatePolicy(RatePolicyDetailModel model)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" RoomPolicyID");
            sql.AppendLine(" FROM RoomPolicy WITH(NOLOCK)");
            sql.AppendLine(" WHERE 1=1");
            sql.AppendLine(" and ((StartDate <= @StartDate and EndDate>=@StartDate)");
            sql.AppendLine(" or (StartDate <= @EndDate and EndDate>=@EndDate))");
            sql.AppendLine(" and RoomType = @RoomType");
            if (model.OpertationStatus == GlobalVariable.UpdateStatus.Update)
            {
                sql.AppendLine(" and RoomPolicyID <> @RoomPolicyID");
            }

            SqlParameter[] paras = new SqlParameter[4];
            paras[0] = new SqlParameter("@StartDate", model.StartDate);
            paras[1] = new SqlParameter("@EndDate", model.EndDate);
            paras[2] = new SqlParameter("@RoomType", model.RoomType);
            paras[3] = new SqlParameter("@RoomPolicyID", model.RoomPolicyID);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            List<RatePolicyDetailModel> ratePolicy = new List<RatePolicyDetailModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }

            return false;
        }

        public RatePolicyDetailModel GetDataByID(string id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" RoomPolicyID, StartDate, EndDate, RoomType,");
            //sql.AppendLine(" RoomTypeName = case RoomType when 0 then 'King' when 1 then 'Queen' end, ");
            sql.AppendLine(" WeekDays, BasicRate, DiscountRate, IsDelete");
            sql.AppendLine(" FROM RoomPolicy WITH(NOLOCK)");
            sql.AppendLine(" WHERE RoomPolicyID=@RoomPolicyID");

            SqlParameter[] paras = new SqlParameter[3];
            paras[0] = new SqlParameter("@RoomPolicyID", id);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            RatePolicyDetailModel ratePolicy = new RatePolicyDetailModel();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                ratePolicy = DataMappingHelper<RatePolicyDetailModel>.DataTableToEntity(ds.Tables[0]);

            }

            return ratePolicy;
        }


    }
}
