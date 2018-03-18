using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


using AFH.Barcaldine.Common;
using AFH.Common.DataBaseAccess;
using AFH.Common.Serializer;
using AFH.Barcaldine.Models;


namespace AFH.Barcaldine.Core
{
    public class ShippingCore
    {
        public List<ShippingResultModel> GetSearchData(ShippingSearchModel search)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" ShippingID, State, ShippingRate, IsDelete");
            sql.AppendLine(" FROM Shipping WITH(NOLOCK)");
            sql.AppendLine(" WHERE 1=1");

            if (search.State != "9")
            {
                sql.AppendLine(" AND State = @AuState");
            }

            if (search.IsDelete != "9")
            {
                sql.AppendLine(" AND IsDelete = @IsDelete");
            }


            SqlParameter[] paras = new SqlParameter[2];
            if (search.State != "9")
            {
                paras[0] = new SqlParameter("@AuState", search.State);
            }
            if (search.IsDelete != "9")
            {
                paras[1] = new SqlParameter("@IsDelete", search.IsDelete);
            }

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            List<ShippingResultModel> shipping = new List<ShippingResultModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                shipping = DataMappingHelper<ShippingResultModel>.DataTableToList(ds.Tables[0]);

            }

            return shipping;

        }

        public ShippingDetailModel GetDataByID(string id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" ShippingID, State, ShippingRate, IsDelete");
            sql.AppendLine(" FROM Shipping WITH(NOLOCK)");
            sql.AppendLine(" WHERE ShippingID=@ShippingID");
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@ShippingID", id);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            ShippingDetailModel Shipping = new ShippingDetailModel();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Shipping = DataMappingHelper<ShippingDetailModel>.DataTableToEntity(ds.Tables[0]);
            }

            return Shipping;
        }

        public bool IsExistState(string state)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" ShippingID");
            sql.AppendLine(" FROM Shipping WITH(NOLOCK)");
            sql.AppendLine(" WHERE State=@State");
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@State", state);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            
            return false;
        }


        public void InsertData(ShippingDetailModel model)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            StringBuilder str = new StringBuilder();
            str.AppendLine("Insert into Shipping");
            str.AppendLine(" (State, ShippingRate, IsDelete, CreateTime, CreateUser, UpdateTime, UpdateUser) ");
            str.AppendLine(" Values (@State, @ShippingRate, @IsDelete, getdate(), @CreateUser,getdate(), @UpdateUser);");

            SqlParameter[] paras = new SqlParameter[5];

            paras[0] = new SqlParameter("@State", Common.VariableConvert.ConvertStringToDBValue(model.State));
            paras[1] = new SqlParameter("@ShippingRate", model.ShippingRate);
            paras[2] = new SqlParameter("@IsDelete", Common.VariableConvert.BitConverter(model.IsDelete));
            paras[3] = new SqlParameter("@CreateUser", Common.VariableConvert.ConvertStringToDBValue(model.CreateUser));
            paras[4] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(model.UpdateUser));

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);
        }

        public void UpdateData(ShippingDetailModel model)
        {

            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            StringBuilder str = new StringBuilder();
            str.AppendLine("update Shipping set");
            str.AppendLine(" State =@State");
            str.AppendLine(" ,ShippingRate =@ShippingRate");
            str.AppendLine(" ,IsDelete =@IsDelete");
            str.AppendLine(" ,UpdateUser =@UpdateUser");
            str.AppendLine(" ,UpdateTime =getdate()");
            str.AppendLine(" where ShippingID=@ShippingID");
            

            SqlParameter[] paras = new SqlParameter[6];

            paras[0] = new SqlParameter("@State", Common.VariableConvert.ConvertStringToDBValue(model.State));
            paras[1] = new SqlParameter("@ShippingRate", model.ShippingRate);
            paras[2] = new SqlParameter("@IsDelete", Common.VariableConvert.BitConverter(model.IsDelete));
            paras[3] = new SqlParameter("@CreateUser", Common.VariableConvert.ConvertStringToDBValue(model.CreateUser));
            paras[4] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(model.UpdateUser));
            paras[5] = new SqlParameter("@ShippingID", model.ShippingID);

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);

        }


        public string GetShipping()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("select [State], ShippingRate from Shipping where IsDelete = 0");

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(str.ToString());

            List<ShippingInfoModel> Shipping = new List<ShippingInfoModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                Shipping = DataMappingHelper<ShippingInfoModel>.DataTableToList(ds.Tables[0]);

                JsonSerializer json = new JsonSerializer();
                return json.Serialize<List<ShippingInfoModel>>(Shipping);
            }
            return string.Empty;
         
        }
    }
}
