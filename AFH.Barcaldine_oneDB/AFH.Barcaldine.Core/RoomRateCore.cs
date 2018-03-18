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
    public class RoomRateCore
    {
        public List<RoomRateResultModel> GetSearchData(RoomRateSearchModel search)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" RoomRateID, RateName, RateType,");
            sql.AppendLine(" RateTypeName = case RateType when 0 then 'Basic Rate' when 1 then 'Discount Rate' end, ");
            sql.AppendLine(" BasicRate, DiscountRate, DiscountDays, IsDelete");
            sql.AppendLine(" FROM RoomRate WITH(NOLOCK)");
            sql.AppendLine(" WHERE 1=1");

            if (!string.IsNullOrEmpty(search.RateName))
            {
                sql.AppendLine(" and RateName like '%' + @RateName + '%'");
            }
            if (search.RateType != 9)
            {
                sql.AppendLine(" and RateType = @RateType");
            }
            if (search.IsDelete != "9")
            {
                sql.AppendLine(" and IsDelete = @IsDelete");
            }


            SqlParameter[] paras = new SqlParameter[3];
            if (!string.IsNullOrEmpty(search.RateName))
            {
                paras[0] = new SqlParameter("@RateName", search.RateName);
            }
            if (search.RateType != 9)
            {
                paras[1] = new SqlParameter("@RateType", search.RateType);
            }
            if (search.IsDelete != "9")
            {
                paras[2] = new SqlParameter("@IsDelete", search.IsDelete);
            }

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            List<RoomRateResultModel> roomRate = new List<RoomRateResultModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                roomRate = DataMappingHelper<RoomRateResultModel>.DataTableToList(ds.Tables[0]);

            }

            return roomRate;

        }

        public RoomRateDetailModel GetDataByID(string id)
        {
            
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" RoomRateID, RateName, RateType,");
            sql.AppendLine(" BasicRate, DiscountRate, DiscountDays, IsDelete");
            sql.AppendLine(" FROM RoomRate WITH(NOLOCK)");
            sql.AppendLine(" WHERE RoomRateID=@RoomRateID");

            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@RoomRateID", id);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            RoomRateDetailModel roomRate = new RoomRateDetailModel();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                roomRate = DataMappingHelper<RoomRateDetailModel>.DataTableToEntity(ds.Tables[0]);

            }

            return roomRate;

        }

        public void InsertData(RoomRateDetailModel model)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            StringBuilder str = new StringBuilder();
            str.AppendLine("Insert into RoomRate");
            str.AppendLine(" (RateName, RateType, BasicRate, DiscountRate, DiscountDays, IsDelete, CreateTime, CreateUser, UpdateTime, UpdateUser) ");
            str.AppendLine(" Values (@RateName, @RateType, @BasicRate, @DiscountRate, @DiscountDays, @IsDelete, getdate(), @CreateUser,getdate(), @UpdateUser);");

            SqlParameter[] paras = new SqlParameter[8];
            paras[0] = new SqlParameter("@RateName", Common.VariableConvert.ConvertStringToDBValue(model.RateName));
            paras[1] = new SqlParameter("@RateType", model.RateType);
            paras[2] = new SqlParameter("@BasicRate", model.BasicRate);
            paras[3] = new SqlParameter("@DiscountRate", model.DiscountRate);
            paras[4] = new SqlParameter("@DiscountDays", model.DiscountDays);
            paras[5] = new SqlParameter("@IsDelete", Common.VariableConvert.BitConverter(model.IsDelete));
            paras[6] = new SqlParameter("@CreateUser", Common.VariableConvert.ConvertStringToDBValue(model.CreateUser));
            paras[7] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(model.UpdateUser));

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);
        }

        public void UpdateData(RoomRateDetailModel model)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            StringBuilder str = new StringBuilder();
            str.AppendLine("update RoomRate set");
            str.AppendLine(" RateName=@RateName");
            str.AppendLine(" ,RateType=@RateType");
            str.AppendLine(" ,BasicRate=@BasicRate");
            str.AppendLine(" ,DiscountRate=@DiscountRate");
            str.AppendLine(" ,DiscountDays=@DiscountDays");
            str.AppendLine(" ,IsDelete=@IsDelete");
            str.AppendLine(" ,UpdateTime=getdate()");
            str.AppendLine(" ,UpdateUser=@UpdateUser");
            str.AppendLine(" where RoomRateID=@RoomRateID");

            SqlParameter[] paras = new SqlParameter[8];
            paras[0] = new SqlParameter("@RateName", Common.VariableConvert.ConvertStringToDBValue(model.RateName));
            paras[1] = new SqlParameter("@RateType", model.RateType);
            paras[2] = new SqlParameter("@BasicRate", model.BasicRate);
            paras[3] = new SqlParameter("@DiscountRate", model.DiscountRate);
            paras[4] = new SqlParameter("@DiscountDays", model.DiscountDays);
            paras[5] = new SqlParameter("@IsDelete", Common.VariableConvert.BitConverter(model.IsDelete));
            paras[6] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(model.UpdateUser));
            paras[7] = new SqlParameter("@RoomRateID", model.RoomRateID);

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);

        }


        public List<RoomRateDetailModel> GetDataByRateType(int rateType)
        {

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" RoomRateID, RateName, RateType,");
            sql.AppendLine(" BasicRate, DiscountRate, DiscountDays, IsDelete");
            sql.AppendLine(" FROM RoomRate WITH(NOLOCK)");
            sql.AppendLine(" WHERE IsDelete = 0 and RateType=@RateType");

            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@RateType", rateType);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            List<RoomRateDetailModel> roomRate = new List<RoomRateDetailModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                roomRate = DataMappingHelper<RoomRateDetailModel>.DataTableToList(ds.Tables[0]);

            }

            return roomRate;

        }


    }
}
