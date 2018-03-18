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
    public class WineCategoryCore
    {
        public List<WineCategoryResultModel> GetSearchData(WineCategorySearchModel search)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" WineCategoryID, WineCategoryName, IsDelete");
            sql.AppendLine(" FROM WineCategory WITH(NOLOCK)");

            
            if (search.IsDelete != "9")
            {
                sql.AppendLine(" WHERE ");
                sql.AppendLine(" IsDelete = @IsDelete");
            }


            SqlParameter[] paras;

            paras = new SqlParameter[1];
            if (search.IsDelete != "9")
            {
                paras[0] = new SqlParameter("@IsDelete", search.IsDelete);
            }

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            List<WineCategoryResultModel> wineCategory = new List<WineCategoryResultModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                wineCategory = DataMappingHelper<WineCategoryResultModel>.DataTableToList(ds.Tables[0]);
                
            }

            return wineCategory;

        }

        public WineCategoryDetailModel GetDataByID(string id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" WineCategoryID, WineCategoryName, ImageUrl, IsDelete");
            sql.AppendLine(" FROM WineCategory WITH(NOLOCK)");
            sql.AppendLine(" WHERE WineCategoryID=@WineCategoryID");
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@WineCategoryID", id);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            WineCategoryDetailModel wineCategory = new WineCategoryDetailModel();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                wineCategory.WineCategoryID = Convert.ToInt32(dt.Rows[0]["WineCategoryID"].ToString());

                JsonSerializer myJson = new JsonSerializer();
                wineCategory.WineCategoryName = myJson.Deserialize<WineCategoryNameModel>(dt.Rows[0]["WineCategoryName"].ToString());

                wineCategory.ImageUrl = dt.Rows[0]["ImageUrl"].ToString();

                wineCategory.IsDelete = Convert.ToBoolean(dt.Rows[0]["IsDelete"]);
            }

            return wineCategory;
        }

        public void InsertData(WineCategoryDetailModel model)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            StringBuilder str = new StringBuilder();
            str.AppendLine("Insert into WineCategory");
            str.AppendLine(" (WineCategoryName, IsDelete, ImageUrl, CreateTime, CreateUser, UpdateTime, UpdateUser) ");
            str.AppendLine(" Values (@WineCategoryName, @IsDelete, @ImageUrl, getdate(), @CreateUser,getdate(), @UpdateUser);");

            SqlParameter[] paras = new SqlParameter[5];

            JsonSerializer myJsonSerializer = new JsonSerializer();
            string wineCategoryName = myJsonSerializer.Serialize<WineCategoryNameModel>(model.WineCategoryName);
            paras[0] = new SqlParameter("@WineCategoryName", Common.VariableConvert.ConvertStringToDBValue(wineCategoryName));
            paras[1] = new SqlParameter("@IsDelete", Common.VariableConvert.BitConverter(model.IsDelete));
            paras[2] = new SqlParameter("@ImageUrl", Common.VariableConvert.ConvertStringToDBValue(model.ImageUrl));
            paras[3] = new SqlParameter("@CreateUser", Common.VariableConvert.ConvertStringToDBValue(model.CreateUser));
            paras[4] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(model.UpdateUser));
            
            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);

        }

        public void UpdateData(WineCategoryDetailModel model)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            StringBuilder str = new StringBuilder();
            str.AppendLine("UPDATE WineCategory SET ");
            str.AppendLine(" WineCategoryName=@WineCategoryName ");
            str.AppendLine(" ,IsDelete=@IsDelete ");
            str.AppendLine(" ,ImageUrl=@ImageUrl ");
            str.AppendLine(" ,UpdateTime=getdate() ");
            str.AppendLine(" ,UpdateUser=@UpdateUser ");
            str.AppendLine(" where WineCategoryID=@WineCategoryID ");


            SqlParameter[] paras = new SqlParameter[5];

            JsonSerializer myJsonSerializer = new JsonSerializer();
            string wineCategoryName = myJsonSerializer.Serialize<WineCategoryNameModel>(model.WineCategoryName);
            paras[0] = new SqlParameter("@WineCategoryName", Common.VariableConvert.ConvertStringToDBValue(wineCategoryName));
            paras[1] = new SqlParameter("@IsDelete", Common.VariableConvert.BitConverter(model.IsDelete));
            paras[2] = new SqlParameter("@ImageUrl", Common.VariableConvert.ConvertStringToDBValue(model.ImageUrl));            
            paras[3] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(model.UpdateUser));
            paras[4] = new SqlParameter("@WineCategoryID", model.WineCategoryID);

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);
        }

        public IEnumerable<SelectListItem> GetCategoryList(string select, bool showAll)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" WineCategoryID, WineCategoryName, IsDelete");
            sql.AppendLine(" FROM WineCategory WITH(NOLOCK)");
            sql.AppendLine(" WHERE IsDelete = 0");

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString());
            List<SelectListItem> items = new List<SelectListItem>();

            if (showAll)
            {
                if (string.IsNullOrEmpty(select))
                {
                    items.Add(new SelectListItem { Text = "All", Value = "-1", Selected = true });
                }
                else
                {
                    items.Add(new SelectListItem { Text = "All", Value = "-1" });
                }
            }

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow dw in dt.Rows)
                {

                    JsonSerializer json = new JsonSerializer();
                    WineCategoryNameModel myWineCategoryNameModel = json.Deserialize<WineCategoryNameModel>(dw["WineCategoryName"].ToString());

                    string id = dw["WineCategoryID"].ToString();
                    if (id == select)
                    {
                        items.Add(new SelectListItem { Text = myWineCategoryNameModel.English, Value = id, Selected = true });
                    }
                    else
                    {
                        items.Add(new SelectListItem { Text = myWineCategoryNameModel.English, Value = id });
                    }
                }
            }

            return items;
        }


        public List<WineCategoryDetailModel> GetCategorys()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" WineCategoryID, WineCategoryName, ImageUrl");
            sql.AppendLine(" FROM WineCategory WITH(NOLOCK)");
            sql.AppendLine(" WHERE IsDelete = 0");

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString());
            List<WineCategoryDetailModel> categorys = new List<WineCategoryDetailModel>();


            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dw in ds.Tables[0].Rows)
                {
                    WineCategoryDetailModel category = new WineCategoryDetailModel();
                    category.WineCategoryID = Convert.ToInt32(dw["WineCategoryID"]);

                    JsonSerializer myJson = new JsonSerializer();
                    category.WineCategoryName = myJson.Deserialize<WineCategoryNameModel>(dw["WineCategoryName"].ToString());

                    category.ImageUrl = dw["ImageUrl"].ToString();

                    categorys.Add(category);
                }
            }

            return categorys;
        }
    }
}
