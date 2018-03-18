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


using AFH.Barcaldine.Models;

namespace AFH.Barcaldine.Core
{
    public class OrderCore
    {
        public OrderListModel GetSearchResult(OrderListModel model)
        {

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" OrderID, OrderNo,");
            sql.AppendLine(" OrderTypeName =  case OrderType when 0 then 'Room' when 1 then 'Wine' end, ");
            sql.AppendLine(" ProcessTypeName =  case ProcessType when 0 then 'Online' when 1 then 'Offline' end, ");
            sql.AppendLine(" OrderDate, ");
            sql.AppendLine(" OriginalPrice, Discount, Shipping, TotalPrice, Tax, ");
            sql.AppendLine(" OrderStatusName =  case OrderStatus when 0 then 'Init' when 1 then 'Success' when 2 then 'Fail' when 3 then 'Exception' end, ");
            sql.AppendLine(" OrderDesc ");
            sql.AppendLine(" FROM [Order] WITH(NOLOCK)");
            sql.AppendLine(" WHERE 1=1");
            if (!string.IsNullOrEmpty(model.OrderListSearch.OrderNo))
            {
                sql.AppendLine(" AND OrderNo LIKE '%' + @OrderNo + '%'");
            }
            if (model.OrderListSearch.OrderType != 9)
            {
                sql.AppendLine(" AND OrderType = @OrderType");
            }
            if (model.OrderListSearch.ProcessType != 9)
            {
                sql.AppendLine(" AND ProcessType = @ProcessType");
            }
            if (model.OrderListSearch.OrderStatus != 9)
            {
                sql.AppendLine(" AND OrderStatus = @OrderStatus");
            }
            if (model.OrderListSearch.OrderDateStart != null)
            {
                sql.AppendLine(" AND OrderDate >= @OrderDateStart");
            }
            if (model.OrderListSearch.OrderDateEnd != null)
            {
                sql.AppendLine(" AND OrderDate <= @OrderDateEnd");
            }
            sql.AppendLine(" ORDER BY OrderDate DESC ");

            SqlParameter[] paras = new SqlParameter[6];
            if (!string.IsNullOrEmpty(model.OrderListSearch.OrderNo))
            {
                paras[0] = new SqlParameter("@OrderNo", model.OrderListSearch.OrderNo);
            }
            if (model.OrderListSearch.OrderType != 9)
            {
                paras[1] = new SqlParameter("@OrderType", model.OrderListSearch.OrderType);
            }
            if (model.OrderListSearch.ProcessType != 9)
            {
                paras[2] = new SqlParameter("@ProcessType", model.OrderListSearch.ProcessType);
            }
            if (model.OrderListSearch.OrderStatus != 9)
            {
                paras[3] = new SqlParameter("@OrderStatus", model.OrderListSearch.OrderStatus);
            }
            if (model.OrderListSearch.OrderDateStart != null)
            {
                paras[4] = new SqlParameter("@OrderDateStart", model.OrderListSearch.OrderDateStart);
            }
            if (model.OrderListSearch.OrderDateEnd != null)
            {
                paras[5] = new SqlParameter("@OrderDateEnd", Convert.ToDateTime(model.OrderListSearch.OrderDateEnd).AddDays(1).AddSeconds(-1));
            }

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            model.OrderListResult = new List<Models.OrderListResultModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.OrderListResult = DataMappingHelper<OrderListResultModel>.DataTableToList(ds.Tables[0]);

            }

            return model;
        }

        public OrderDetailModel GetOrderDetail(string orderID)
        {

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");

            //Order
            sql.AppendLine(" O.OrderID, O.OrderNo,");
            sql.AppendLine(" OrderType, OrderTypeName =  case O.OrderType when 0 then 'Room' when 1 then 'Wine' end, ");
            sql.AppendLine(" O.ProcessType, ProcessTypeName =  case O.ProcessType when 0 then 'Online' when 1 then 'Offline' end, ");
            sql.AppendLine(" O.OrderDate, ");
            sql.AppendLine(" O.OriginalPrice, O.Discount, O.Shipping, O.TotalPrice, O.Tax, ");
            sql.AppendLine(" O.OrderStatus, OrderStatusName =  case O.OrderStatus when 0 then 'Init' when 1 then 'Success' when 2 then 'Fail' when 3 then 'Exception' end, ");
            sql.AppendLine(" O.OrderDesc, ");

            //Customer
            sql.AppendLine(" C.Name, C.Phone, C.Email, C.Birthday, C.Address, C.State, C.PostCode,");

            //PaymentTransaction
            sql.AppendLine(" P.RequestID, P.ResponseID, P.ResponseStatus, P.ResponseMsg,");

            //FaultOrders
            sql.AppendLine(" F.FaultMessage, F.IsDelete");

            sql.AppendLine(" FROM [Order] O WITH(NOLOCK)");
            sql.AppendLine(" LEFT JOIN Customer C WITH(NOLOCK) on O.OrderID = C.OrderID ");
            sql.AppendLine(" LEFT JOIN PaymentTransaction P WITH(NOLOCK) on O.OrderID = P.OrderID ");
            sql.AppendLine(" LEFT JOIN FaultOrders F WITH(NOLOCK) on O.OrderID = F.OrderID ");
            sql.AppendLine(" WHERE O.OrderID = @OrderID");

            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@OrderID", Convert.ToInt32(orderID));

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            OrderDetailModel model = new Models.OrderDetailModel();

            model.Customer = new Models.CustomerModel();
            model.PaymentTransaction = new Models.PaymentTransactionModel();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dwOrder= ds.Tables[0].Rows[0];
                model.OrderID = Convert.ToInt32(dwOrder["OrderID"]);

                //order
                model.OrderNo = dwOrder["OrderNo"].ToString();
                model.OrderType = Convert.ToInt32(dwOrder["OrderType"]);
                model.OrderTypeName = dwOrder["OrderTypeName"].ToString();
                model.ProcessType = Convert.ToInt32(dwOrder["ProcessType"]);
                model.ProcessTypeName = dwOrder["ProcessTypeName"].ToString();
                model.OrderDate = Convert.ToDateTime(dwOrder["OrderDate"]);
                model.OriginalPrice = Convert.ToDouble(dwOrder["OriginalPrice"]);
                model.Discount = Convert.ToDouble(dwOrder["Discount"]);
                model.Shipping = Convert.ToDouble(dwOrder["Shipping"]);
                model.TotalPrice = Convert.ToDouble(dwOrder["TotalPrice"]);
                model.Tax = Convert.ToDouble(dwOrder["Tax"]);
                model.OrderStatus = Convert.ToInt32(dwOrder["OrderStatus"]);
                model.OrderStatusName = dwOrder["OrderStatusName"].ToString();
                model.OrderDesc = dwOrder["OrderDesc"].ToString();

                //customer
                model.Customer.Name = dwOrder["Name"].ToString();
                model.Customer.Phone = dwOrder["Phone"].ToString();
                model.Customer.Email = dwOrder["Email"].ToString();
                if (dwOrder["Birthday"] == DBNull.Value)
                {
                    model.Customer.Birthday = null;
                }
                else
                {
                    model.Customer.Birthday = Convert.ToDateTime(dwOrder["Birthday"]);
                }
                model.Customer.Address = dwOrder["Address"].ToString();
                model.Customer.State = dwOrder["State"].ToString();
                model.Customer.PostCode = dwOrder["PostCode"].ToString();

                //PaymentTransaction
                model.PaymentTransaction.RequestID = dwOrder["RequestID"].ToString();
                model.PaymentTransaction.ResponseID = dwOrder["ResponseID"].ToString();
                model.PaymentTransaction.ResponseStatus = dwOrder["ResponseStatus"].ToString();
                model.PaymentTransaction.ResponseMsg = dwOrder["ResponseMsg"].ToString();

                //Order detail
                if(model.OrderType == (int)GlobalVariable.OrderType.Room)
                {
                    //Room
                    model.RoomOrderDetail = this.GetRoomOrderDetail(orderID);
                }
                else 
                {
                    //Wine
                    model.WineOrderDetail = this.GetWineOrderDetail(orderID);
                }

                //log
                model.Log = this.GetLog(orderID);

                //FaultOrders
                model.FaultOrder = new Models.FaultOrderModel();
                model.FaultOrder.FaultMessage = dwOrder["FaultMessage"].ToString();
                if (dwOrder["IsDelete"] != DBNull.Value)
                {
                    model.FaultOrder.IsDelete = Convert.ToBoolean(dwOrder["IsDelete"]);
                }
            }


            return model;
        }

        private List<RoomOrderDetailModel> GetRoomOrderDetail(string orderID)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" RoomType = case RoomType when 0 then 'King Room' when 1 then 'Queen Room' end, CheckinDate, CheckoutDate, ");
            sql.AppendLine(" Price, Discount, Quantity, TotalPrice, ");
            sql.AppendLine(" AdultCount, KidsCount, AdditionalDetail");
            sql.AppendLine(" FROM [RoomOrderDetail] WITH(NOLOCK)");
            sql.AppendLine(" WHERE OrderID = @OrderID");
            sql.AppendLine(" order by RoomOrderDetailID asc");


            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@OrderID", Convert.ToInt32(orderID));

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            List<RoomOrderDetailModel> detail = new List<Models.RoomOrderDetailModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                detail = DataMappingHelper<RoomOrderDetailModel>.DataTableToList(ds.Tables[0]);
            }
            return detail;
        }

        private List<WineOrderDetailModel> GetWineOrderDetail(string orderID)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" W.WineID, W.Quantity, ");
            sql.AppendLine(" W.Price, W.Discount, W.TotalPrice, ");
            sql.AppendLine(" P.ProductYear, P.Describle ");
            sql.AppendLine(" FROM [WineOrderDetail] W WITH(NOLOCK)");
            sql.AppendLine(" left join WineProduct P WITH(NOLOCK) on W.WineID = P.WineID");
            sql.AppendLine(" WHERE W.OrderID = @OrderID");
            sql.AppendLine(" order by W.WineID asc");


            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@OrderID", Convert.ToInt32(orderID));

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            List<WineOrderDetailModel> detailList = new List<Models.WineOrderDetailModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                //detail = DataMappingHelper<WineOrderDetailModel>.DataTableToList(ds.Tables[0]);
                foreach (DataRow dw in ds.Tables[0].Rows)
                {
                    WineOrderDetailModel detail = new Models.WineOrderDetailModel();
                    detail.WineID = Convert.ToInt32(dw["WineID"]);

                    JsonSerializer json = new JsonSerializer();
                    ProductDescribleModel desc = json.Deserialize<List<ProductDescribleModel>>(dw["Describle"].ToString()).Find(m=>m.Language == GlobalVariable.LanguageName.English);
                    detail.WineName = desc.DescribleDetail.Name;

                    detail.Quantity = Convert.ToInt32(dw["Quantity"]);

                    detail.Price = Convert.ToDouble(dw["Price"]);
                    detail.Discount = Convert.ToDouble(dw["Discount"]);
                    detail.TotalPrice = Convert.ToDouble(dw["TotalPrice"]);

                    detailList.Add(detail);
                }
            }
            return detailList;
        }

        private List<LogModel> GetLog(string orderID)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" LogContent, ErrorMessage, LogTime, System, SystemModel");
            sql.AppendLine(" FROM [LogInfo] WITH(NOLOCK)");
            sql.AppendLine(" WHERE ReferenceID = @OrderID");
            sql.AppendLine(" order by LogTime desc");


            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@OrderID", Convert.ToInt32(orderID));

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            List<LogModel> log = new List<Models.LogModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                log = DataMappingHelper<LogModel>.DataTableToList(ds.Tables[0]);
            }

            return log;
        }

        public FaultOrderListModel GetFaultSearchResult(FaultOrderListModel model)
        {

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" O.OrderID, O.OrderNo,");
            sql.AppendLine(" OrderTypeName =  case O.OrderType when 0 then 'Room' when 1 then 'Wine' end, ");
            sql.AppendLine(" ProcessTypeName =  case O.ProcessType when 0 then 'Online' when 1 then 'Offline' end, ");
            sql.AppendLine(" O.OrderDate, ");
            sql.AppendLine(" O.OriginalPrice, O.Discount, O.Shipping, O.TotalPrice, O.Tax, ");
            sql.AppendLine(" OrderStatusName =  case O.OrderStatus when 0 then 'Init' when 1 then 'Success' when 2 then 'Fail' when 3 then 'Exception' end, ");
            sql.AppendLine(" O.OrderDesc, ");
            sql.AppendLine(" F.FaultMessage, F.IsDelete");   
            sql.AppendLine(" FROM FaultOrders F WITH(NOLOCK)");
            sql.AppendLine(" left join [Order] O WITH(NOLOCK) on O.OrderID = F.OrderID");
            sql.AppendLine(" WHERE 1=1");
            if (!string.IsNullOrEmpty(model.FaultOrderListSearch.OrderNo))
            {
                sql.AppendLine(" AND O.OrderNo LIKE '%' + @OrderNo + '%'");
            }
            if (model.FaultOrderListSearch.OrderType != 9)
            {
                sql.AppendLine(" AND O.OrderType = @OrderType");
            }
            if (model.FaultOrderListSearch.ProcessType != 9)
            {
                sql.AppendLine(" AND O.ProcessType = @ProcessType");
            }
            if (model.FaultOrderListSearch.OrderStatus != 9)
            {
                sql.AppendLine(" AND O.OrderStatus = @OrderStatus");
            }
            if (model.FaultOrderListSearch.OrderDateStart != null)
            {
                sql.AppendLine(" AND O.OrderDate >= @OrderDateStart");
            }
            if (model.FaultOrderListSearch.OrderDateEnd != null)
            {
                sql.AppendLine(" AND O.OrderDate <= @OrderDateEnd");
            }
            if (model.FaultOrderListSearch.IsDelete != 9)
            {
                sql.AppendLine(" AND F.IsDelete = @IsDelete");
            }
            sql.AppendLine(" ORDER BY O.OrderDate, O.OrderStatus DESC ");



            SqlParameter[] paras = new SqlParameter[7];
            if (!string.IsNullOrEmpty(model.FaultOrderListSearch.OrderNo))
            {
                paras[0] = new SqlParameter("@OrderNo", model.FaultOrderListSearch.OrderNo);
            }
            if (model.FaultOrderListSearch.OrderType != 9)
            {
                paras[1] = new SqlParameter("@OrderType", model.FaultOrderListSearch.OrderType);
            }
            if (model.FaultOrderListSearch.ProcessType != 9)
            {
                paras[2] = new SqlParameter("@ProcessType", model.FaultOrderListSearch.ProcessType);
            }
            if (model.FaultOrderListSearch.OrderStatus != 9)
            {
                paras[3] = new SqlParameter("@OrderStatus", model.FaultOrderListSearch.OrderStatus);
            }
            if (model.FaultOrderListSearch.OrderDateStart != null)
            {
                paras[4] = new SqlParameter("@OrderDateStart", model.FaultOrderListSearch.OrderDateStart);
            }
            if (model.FaultOrderListSearch.OrderDateEnd != null)
            {
                paras[5] = new SqlParameter("@OrderDateEnd", Convert.ToDateTime(model.FaultOrderListSearch.OrderDateEnd).AddDays(1).AddSeconds(-1));
            }
            if (model.FaultOrderListSearch.IsDelete != 9)
            {
                paras[6] = new SqlParameter("@IsDelete", model.FaultOrderListSearch.IsDelete);
            }

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            model.FaultOrderListResult = new List<Models.FaultOrderListResultModel>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.FaultOrderListResult = DataMappingHelper<FaultOrderListResultModel>.DataTableToList(ds.Tables[0]);

            }

            return model;
        }

        

    }
}
