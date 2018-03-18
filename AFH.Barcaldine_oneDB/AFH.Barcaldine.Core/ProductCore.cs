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
    public class ProductCore
    {
        public List<ProductResultModel> GetSearch(ProductSearchModel search)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" C.WineCategoryName, ");
            sql.AppendLine(" P.WineID, P.WineCategoryID, P.ProductYear, P.Price, P.Describle, P.IsDelete");
            sql.AppendLine(" FROM WineProduct P WITH(NOLOCK)");
            sql.AppendLine(" LEFT JOIN WineCategory C WITH(NOLOCK) ON P.WineCategoryID = C.WineCategoryID");
            sql.AppendLine(" WHERE 1=1");

            if (search.CategoryID != -1)
            {
                sql.AppendLine(" AND C.WineCategoryID = @WineCategoryID");
            }
            if (search.IsDelete != "9")
            {
                sql.AppendLine(" AND P.IsDelete = @IsDelete");
            }


            SqlParameter[] paras;

            paras = new SqlParameter[2];
            if (search.CategoryID != -1)
            {
                paras[0] = new SqlParameter("@WineCategoryID", search.CategoryID);
            }
            if (search.IsDelete != "9")
            {
                paras[1] = new SqlParameter("@IsDelete", search.IsDelete);
            }

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            List<ProductResultModel> product = new List<ProductResultModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                product = DataMappingHelper<ProductResultModel>.DataTableToList(ds.Tables[0]);

            }

            return product;
        }

        /// <summary>
        /// offline
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProductDetailModel GetWineByID(string id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" P.WineID, P.WineCategoryID, P.ProductYear, P.Price, P.Describle, P.Region, P.IsDelete");
            sql.AppendLine(" FROM WineProduct P WITH(NOLOCK)");
            sql.AppendLine(" WHERE WineID = @WineID");

            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@WineID", id);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            ProductDetailModel product = new ProductDetailModel();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dw = ds.Tables[0].Rows[0];
                //product = DataMappingHelper<ProductDetailModel>.DataTableToEntity(ds.Tables[0]);
                product.WineID = Convert.ToInt32(dw["WineID"]);
                product.WineCategoryID = Convert.ToInt32(dw["WineCategoryID"]);
                product.ProductYear = Convert.ToInt32(dw["ProductYear"]);
                product.Price = Convert.ToDouble(dw["Price"]);

                JsonSerializer myJson = new JsonSerializer();
                product.Describle = myJson.Deserialize<List<ProductDescribleModel>>(dw["Describle"].ToString());

                product.Region = dw["Region"].ToString();
                product.IsDelete = Convert.ToBoolean(dw["IsDelete"]);


                product.WineImages = this.GetWineImageByWindID(dw["WineID"].ToString());

            }

            return product;
        }

        public List<WineImageModel> GetWineImageByWindID(string wineID)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" WineImageID, WineID, Url, ImageType ");
            sql.AppendLine(" FROM WineImage WITH(NOLOCK)");
            sql.AppendLine(" WHERE WineID = @WineID");
            sql.AppendLine(" ORDER BY ImageType ASC");


            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@WineID", wineID);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);
            List<WineImageModel> wineImage = new List<WineImageModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                wineImage = DataMappingHelper<WineImageModel>.DataTableToList(ds.Tables[0]);
            }
            return wineImage;
        }

        /// <summary>
        /// get new wine id
        /// </summary>
        /// <returns></returns>
        private int GetWineID()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("Insert into WineIDInfo (CreateTime) values (getdate()); ");
            str.AppendLine(" Select scope_identity();");

            SqlAccess mySqlAccess = new SqlAccess();
            int newsID = mySqlAccess.ExecuteScalar(str.ToString());

            return newsID;
        }

        public void InsertData(ProductDetailModel model)
        {

            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            int wineID = this.GetWineID();

            #region WineProduct

            StringBuilder str = new StringBuilder();
            str.AppendLine("Insert into WineProduct");
            str.AppendLine(" (WineID, WineCategoryID, ProductYear, Price, Describle, Region, IsDelete, CreateTime, CreateUser, UpdateTime, UpdateUser) ");
            str.AppendLine(" Values (@WineID, @WineCategoryID, @ProductYear, @Price, @Describle, @Region, @IsDelete, getdate(), @CreateUser,getdate(), @UpdateUser);");

            SqlParameter[] paras = new SqlParameter[9];

            paras[0] = new SqlParameter("@WineID", wineID);
            paras[1] = new SqlParameter("@WineCategoryID", model.WineCategoryID);
            paras[2] = new SqlParameter("@ProductYear", model.ProductYear);
            paras[3] = new SqlParameter("@Price", model.Price);

            JsonSerializer myJsonSerializer = new JsonSerializer();
            string describle = myJsonSerializer.Serialize<List<ProductDescribleModel>>(model.Describle);
            paras[4] = new SqlParameter("@Describle", describle);

            paras[5] = new SqlParameter("@Region", Common.VariableConvert.ConvertStringToDBValue(model.Region));
            paras[6] = new SqlParameter("@IsDelete", Common.VariableConvert.BitConverter(model.IsDelete));

            paras[7] = new SqlParameter("@CreateUser", Common.VariableConvert.ConvertStringToDBValue(model.CreateUser));
            paras[8] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(model.UpdateUser));

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            #endregion

            #region WineImage

            for (int i = 0; i < model.WineImages.Count; i++)
            {
                str = new StringBuilder();
                str.AppendLine("Insert into WineImage");
                str.AppendLine(" (WineID, Url, ImageType) ");
                str.AppendLine(" Values (@WineID, @Url, @ImageType);");

                paras = new SqlParameter[3];
                paras[0] = new SqlParameter("@WineID", wineID);
                paras[1] = new SqlParameter("@Url", Common.VariableConvert.ConvertStringToDBValue(model.WineImages[i].Url));
                paras[2] = new SqlParameter("@ImageType", model.WineImages[i].ImageType);

                sqls.Add(str.ToString());
                cmdParms.Add(paras);
            }

            #endregion

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);

        }

        public void UpdateData(ProductDetailModel model)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            #region WineProduct

            StringBuilder str = new StringBuilder();
            str.AppendLine("Update WineProduct set");
            str.AppendLine(" WineCategoryID =@WineCategoryID ");
            str.AppendLine(" ,ProductYear=@ProductYear");
            str.AppendLine(" ,Price=@Price");
            str.AppendLine(" ,Describle=@Describle");
            str.AppendLine(" ,Region=@Region");
            str.AppendLine(" ,IsDelete=@IsDelete");
            str.AppendLine(" ,UpdateUser=@UpdateUser");
            str.AppendLine(" ,UpdateTime=getdate()");


            str.AppendLine(" Where WineID = @WineID");

            SqlParameter[] paras = new SqlParameter[8];

            paras[0] = new SqlParameter("@WineID", model.WineID);
            paras[1] = new SqlParameter("@WineCategoryID", model.WineCategoryID);
            paras[2] = new SqlParameter("@ProductYear", model.ProductYear);
            paras[3] = new SqlParameter("@Price", model.Price);

            JsonSerializer myJsonSerializer = new JsonSerializer();
            string describle = myJsonSerializer.Serialize<List<ProductDescribleModel>>(model.Describle);
            paras[4] = new SqlParameter("@Describle", describle);

            paras[5] = new SqlParameter("@Region", Common.VariableConvert.ConvertStringToDBValue(model.Region));
            paras[6] = new SqlParameter("@IsDelete", Common.VariableConvert.BitConverter(model.IsDelete));

            paras[7] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(model.UpdateUser));

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            #endregion

            #region WineImage

            //delete all data
            str = new StringBuilder();
            str.AppendLine("Delete from WineImage where WineID = @WineID");

            paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@WineID", model.WineID);

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            //insert new data
            for (int i = 0; i < model.WineImages.Count; i++)
            {
                str = new StringBuilder();
                str.AppendLine("Insert into WineImage");
                str.AppendLine(" (WineID, Url, ImageType) ");
                str.AppendLine(" Values (@WineID, @Url, @ImageType);");

                paras = new SqlParameter[3];
                paras[0] = new SqlParameter("@WineID", model.WineID);
                paras[1] = new SqlParameter("@Url", Common.VariableConvert.ConvertStringToDBValue(model.WineImages[i].Url));
                paras[2] = new SqlParameter("@ImageType", model.WineImages[i].ImageType);

                sqls.Add(str.ToString());
                cmdParms.Add(paras);
            }

            #endregion

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);
        }

        /// <summary>
        /// online: get wine product list/detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ProductView> GetProducts(string id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" C.WineCategoryName, ");
            sql.AppendLine(" P.WineID, P.WineCategoryID, P.ProductYear, P.Price, P.Describle, P.Region, P.IsDelete");
            sql.AppendLine(" FROM WineProduct P WITH(NOLOCK)");
            sql.AppendLine(" LEFT JOIN WineCategory C WITH(NOLOCK) ON P.WineCategoryID = C.WineCategoryID");
            sql.AppendLine(" WHERE P.IsDelete = 0");

            if (!string.IsNullOrEmpty(id))
            {
                sql.AppendLine(" AND P.WineID = @WineID");
            }

            SqlParameter[] paras = new SqlParameter[1];
            if (!string.IsNullOrEmpty(id))
            {
                paras[0] = new SqlParameter("@WineID", id);
            }


            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            List<ProductView> productList = new List<ProductView>();
            JsonSerializer json = new JsonSerializer();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dw in ds.Tables[0].Rows)
                {
                    ProductView product = new ProductView();
                    product.WineID = Convert.ToInt32(dw["WineID"].ToString());

                    product.WineCategoryID = Convert.ToInt32(dw["WineCategoryID"].ToString());
                    product.WineCategoryName = json.Deserialize<WineCategoryNameModel>(dw["WineCategoryName"].ToString());

                    product.ProductYear = Convert.ToInt32(dw["ProductYear"].ToString());
                    product.Price = Convert.ToDouble(dw["Price"].ToString());

                    product.ProductDescrible = json.Deserialize<List<ProductDescribleModel>>(dw["Describle"].ToString());

                    product.Region = dw["Region"].ToString();


                    List<WineImageModel> wineImages = this.GetWineImageByWindID(dw["WineID"].ToString());
                    product.ListPicture = wineImages.Find(m => m.ImageType == GlobalVariable.WineImageType.List).Url;
                    product.DetailPicture = wineImages.Find(m => m.ImageType == GlobalVariable.WineImageType.Detail).Url;

                    productList.Add(product);

                }
            }

            return productList;

        }

        /// <summary>
        /// Online
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WineDetailModel GetWineDetailByID(string id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" C.WineCategoryName, ");
            sql.AppendLine(" P.WineID, P.WineCategoryID, P.ProductYear, P.Price, P.Describle, P.Region, P.IsDelete");
            sql.AppendLine(" FROM WineProduct P WITH(NOLOCK)");
            sql.AppendLine(" LEFT JOIN WineCategory C WITH(NOLOCK) ON P.WineCategoryID = C.WineCategoryID");
            sql.AppendLine(" WHERE P.IsDelete = 0 and P.WineID = @WineID");

            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@WineID", id);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            WineDetailModel product = new WineDetailModel();
            JsonSerializer myJson = new JsonSerializer();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dw = ds.Tables[0].Rows[0];
                product.WineID = Convert.ToInt32(dw["WineID"]);

                product.WineCategoryID = Convert.ToInt32(dw["WineCategoryID"]);
                product.WineCategoryName = myJson.Deserialize<WineCategoryNameModel>(dw["WineCategoryName"].ToString());

                product.ProductYear = Convert.ToInt32(dw["ProductYear"]);
                product.Price = Convert.ToDouble(dw["Price"]);


                List<ProductDescribleModel> describles = new List<ProductDescribleModel>();
                describles = myJson.Deserialize<List<ProductDescribleModel>>(dw["Describle"].ToString());
                foreach (ProductDescribleModel describle in describles)
                {
                    if (describle.Language == GlobalVariable.LanguageName.English)
                    {
                        product.Describle = describle;
                        break;
                    }
                }

                product.Region = dw["Region"].ToString();

                List<WineImageModel> wineImages = this.GetWineImageByWindID(dw["WineID"].ToString());
                product.DetailPicture = wineImages.Find(m => m.ImageType == GlobalVariable.WineImageType.Detail).Url;


            }

            return product;

        }

        /// <summary>
        /// Online
        /// </summary>
        /// <returns></returns>
        public List<WineInfo> GetWineInfo()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" P.WineID, P.WineCategoryID, P.Describle");
            sql.AppendLine(" FROM WineProduct P WITH(NOLOCK)");
            sql.AppendLine(" WHERE P.IsDelete = 0");

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString());

            JsonSerializer myJson = new JsonSerializer();
            List<WineInfo> wineInfos = new List<WineInfo>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow dw in ds.Tables[0].Rows)
                {
                    WineInfo wineInfo = new WineInfo();
                    wineInfo.WineID = Convert.ToInt32(dw["WineID"]);
                    wineInfo.WineCategoryID = Convert.ToInt32(dw["WineCategoryID"]);

                    List<ProductDescribleModel> describles = new List<ProductDescribleModel>();
                    describles = myJson.Deserialize<List<ProductDescribleModel>>(dw["Describle"].ToString());
                    foreach (ProductDescribleModel describle in describles)
                    {
                        if (describle.Language == GlobalVariable.LanguageName.English)
                        {
                            wineInfo.WineName = describle.DescribleDetail.Name;
                            break;
                        }
                    }

                    wineInfos.Add(wineInfo);
                }

            }
            return wineInfos;
        }

        /// <summary>
        /// online--wineorder
        /// </summary>
        /// <returns></returns>
        public List<WineListModel> GetWineInfoForOrder()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" P.WineID, P.ProductYear, P.Price, P.Describle");
            sql.AppendLine(" FROM WineProduct P WITH(NOLOCK)");
            sql.AppendLine(" WHERE P.IsDelete = 0");

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString());

            List<WineListModel> wines = new List<WineListModel>();
            JsonSerializer json = new JsonSerializer();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dw in ds.Tables[0].Rows)
                {
                    WineListModel wine = new WineListModel();
                    wine.WineID = Convert.ToInt32(dw["WineID"]);
                    wine.ProductYear = Convert.ToInt32(dw["ProductYear"]);
                    wine.Price = Convert.ToDouble(dw["Price"]);

                    List<ProductDescribleModel> lstDesc = json.Deserialize<List<ProductDescribleModel>>(dw["Describle"].ToString());

                    foreach (ProductDescribleModel desc in lstDesc)
                    {
                        if (desc.Language == GlobalVariable.LanguageName.English)
                        {
                            wine.WineName = desc.DescribleDetail.Name;
                            break;
                        }
                    }


                    wines.Add(wine);
                }
            }
            return wines;
        }
    }
}
