using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using AFH.Barcaldine.Models;
using AFH.Barcaldine.Common;
using AFH.Common.DataBaseAccess;
using AFH.Common.Serializer;

using PayPal.Payments.Common;
using PayPal.Payments.Common.Utility;
using PayPal.Payments.DataObjects;
using PayPal.Payments.Transactions;

namespace AFH.Barcaldine.Core
{
    public class WineOrderCore
    {


        public WineOrderModel SaveOrder(WineOrderModel order)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();
            JsonSerializer json = new JsonSerializer();

            if (order.OrderID == 0)
            {
                int orderID = this.GetNewOrderID();
                order.OrderID = orderID;
            }

            StringBuilder str;
            SqlParameter[] paras;
     
            #region Customer

            str = new StringBuilder();
            str.AppendLine("Insert into Customer");
            str.AppendLine(" (OrderID, Name, Phone, Email, Birthday, Address, State, PostCode, CreateTime, UpdateTime) ");
            str.AppendLine(" Values (@OrderID, @Name, @Phone, @Email, @Birthday, @Address, @State, @PostCode, Getdate(), getdate());");

            paras = new SqlParameter[8];
            paras[0] = new SqlParameter("@OrderID", order.OrderID);
            paras[1] = new SqlParameter("@Name", order.Delivery.Name);
            paras[2] = new SqlParameter("@Phone", order.Delivery.Phone);
            paras[3] = new SqlParameter("@Email", order.Delivery.Email);
            paras[4] = new SqlParameter("@Birthday", order.Delivery.Birthday);
            paras[5] = new SqlParameter("@Address", order.Delivery.Address);
            paras[6] = new SqlParameter("@State", order.Delivery.State);
            paras[7] = new SqlParameter("@PostCode", order.Delivery.PostCode);

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            #endregion

            #region WineOrderDetail
            order.OriginalPrice = 0;
            foreach (WineListModel detail in order.Wines)
            {
                if (detail.Bottle > 0)
                {
                    str = new StringBuilder();
                    str.AppendLine("Insert into WineOrderDetail");
                    str.AppendLine(" (OrderID, WineID, Quantity, Price, TotalPrice, CreateTime, UpdateTime) ");
                    str.AppendLine(" Values (@OrderID, @WineID, @Quantity, @Price, @TotalPrice, getdate(), getdate());");

                    paras = new SqlParameter[5];
                    paras[0] = new SqlParameter("@OrderID", order.OrderID);
                    paras[1] = new SqlParameter("@WineID", detail.WineID);
                    paras[2] = new SqlParameter("@Quantity", detail.Bottle);
                    paras[3] = new SqlParameter("@Price", detail.Price);

                    double totalDetailPrice = detail.Bottle * detail.Price;
                    order.OriginalPrice += totalDetailPrice;
                    paras[4] = new SqlParameter("@TotalPrice", totalDetailPrice);

                    sqls.Add(str.ToString());
                    cmdParms.Add(paras);
                }


            }

            #endregion

            #region Order

            str = new StringBuilder();
            str.AppendLine("Insert into [Order]");
            str.AppendLine(" (OrderID, OrderType, OrderNo, ProcessType, OrderDate, OriginalPrice, Shipping, TotalPrice, Tax, OrderStatus, CreateTime, UpdateTime) ");
            str.AppendLine(" Values (@OrderID, @OrderType, @OrderNo, @ProcessType, @OrderDate, @OriginalPrice, @Shipping, @TotalPrice, @Tax, @OrderStatus, Getdate(), getdate());");

            paras = new SqlParameter[10];

            paras[0] = new SqlParameter("@OrderID", order.OrderID);
            paras[1] = new SqlParameter("@OrderType", (int)GlobalVariable.OrderType.Wine);
            order.OrderNo = "W" + order.OrderID.ToString().PadLeft(9, '0');
            paras[2] = new SqlParameter("@OrderNo", order.OrderNo);
            paras[3] = new SqlParameter("@ProcessType", order.ProcessType);
            order.OrderDate = DateTime.Now;
            paras[4] = new SqlParameter("@OrderDate", order.OrderDate);

            paras[5] = new SqlParameter("@OriginalPrice", order.OriginalPrice);
            paras[6] = new SqlParameter("@Shipping", order.Shipping);
            order.TotalOrderPrice = order.OriginalPrice + order.Shipping;
            paras[7] = new SqlParameter("@TotalPrice", order.TotalOrderPrice);

            double tax = order.TotalOrderPrice * Convert.ToDouble(ConfigurationManager.AppSettings["TaxRate"].ToString());
            paras[8] = new SqlParameter("@Tax", tax);

            if (order.ProcessType == (int)GlobalVariable.ProcessType.Online)
            {
                int status = (int)GlobalVariable.OrderStatus.Init;
                paras[9] = new SqlParameter("@OrderStatus", status);
            }
            else if (order.ProcessType == (int)GlobalVariable.ProcessType.Offline)
            {
                int status = (int)GlobalVariable.OrderStatus.Success;
                paras[9] = new SqlParameter("@OrderStatus", status);
            }


            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            #endregion

            #region Transaction

            if (order.ProcessType == (int)GlobalVariable.ProcessType.Online)
            {
                string requestID = PayflowUtility.RequestId;
                order.RequestID = requestID;
                str = new StringBuilder();
                str.AppendLine("Insert into [PaymentTransaction]");
                str.AppendLine(" (RequestID, OrderID, CreateTime, UpdateTime) ");
                str.AppendLine(" Values (@RequestID, @OrderID, Getdate(), getdate());");

                paras = new SqlParameter[2];

                paras[0] = new SqlParameter("@RequestID", requestID);
                paras[1] = new SqlParameter("@OrderID", order.OrderID);

                sqls.Add(str.ToString());
                cmdParms.Add(paras);
            }

            #endregion

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);

            return order;
        }

        private int GetNewOrderID()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("Insert into OrderIDInfo (CreateTime) values (getdate()); ");
            str.AppendLine(" Select scope_identity();");

            SqlAccess mySqlAccess = new SqlAccess();
            int orderID = mySqlAccess.ExecuteScalar(str.ToString());

            return orderID;
        }

        public WineOrderModel SaveOrderResult(WineOrderModel order)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            StringBuilder str;
            SqlParameter[] paras;

            #region Order

            str = new StringBuilder();
            str.AppendLine("update [Order] set");
            str.AppendLine(" OrderStatus=@OrderStatus ");
            str.AppendLine(" ,OrderDesc=@OrderDesc ");
            str.AppendLine(" ,UpdateTime = getdate()");
            str.AppendLine(" where orderID = @OrderID ");

            paras = new SqlParameter[3];
            paras[0] = new SqlParameter("@OrderStatus", order.ResultStatus);
            paras[1] = new SqlParameter("@OrderDesc", order.ResultMsg);
            paras[2] = new SqlParameter("@OrderID", order.OrderID);

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            #endregion 


            #region PaymentTransaction

            if (order.Response != null)
            {
                str = new StringBuilder();
                str.AppendLine("update PaymentTransaction set");
                str.AppendLine(" ResponseID=@ResponseID ");
                str.AppendLine(" ,ResponseStatus=@ResponseStatus ");
                str.AppendLine(" ,ResponseMsg=@ResponseMsg ");
                str.AppendLine(" ,UpdateTime = getdate()");
                str.AppendLine(" where RequestID = @RequestID ");

                paras = new SqlParameter[4];
                paras[0] = new SqlParameter("@ResponseID", Common.VariableConvert.ConvertStringToDBValue(order.Response.ResponseID));
                paras[1] = new SqlParameter("@ResponseStatus", order.Response.ResultCode);
                paras[2] = new SqlParameter("@ResponseMsg", Common.VariableConvert.ConvertStringToDBValue(order.Response.ResultMessage));
                paras[3] = new SqlParameter("@RequestID", order.RequestID);

                sqls.Add(str.ToString());
                cmdParms.Add(paras);
            }

            #endregion

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);

            return order;
        }

    }
}
