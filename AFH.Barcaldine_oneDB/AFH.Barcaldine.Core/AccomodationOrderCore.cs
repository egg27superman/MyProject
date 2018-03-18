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
    public class AccomodationOrderCore
    {

        #region calc room rate
       
        public AccomodationOrderModel  CalcRoomRate(AccomodationOrderModel order)
        {
            
            List<AccomodationDiscountPolicy> allDiscountRate = this.GetAllDiscountRate();

            List <AccomodationPolicy> roomPolicy = this.GetRoomPolicy(Convert.ToDateTime(order.CheckInDate), Convert.ToDateTime(order.CheckOutDate), allDiscountRate);

            DateTime startDate = Convert.ToDateTime(order.CheckInDate);
            DateTime endDate = Convert.ToDateTime(order.CheckOutDate);

         // TimeSpan ts = startDate.Subtract(endDate).Duration();
            TimeSpan ts = endDate - startDate;
            int totalstaydays = ts.Days;

            //calc basic Rate
            for (DateTime dt = startDate; dt < endDate; dt = dt.AddDays(1))
            {
                int weekDays = (int)dt.DayOfWeek;
               // string weekDays = dt.ToString();

                roomPolicy = this.CalcBasicRate(roomPolicy, weekDays.ToString(), GlobalVariable.RoomType.King);
                roomPolicy = this.CalcBasicRate(roomPolicy, weekDays.ToString(), GlobalVariable.RoomType.Queen);
            }

            //calc discount Rate
            roomPolicy = this.CalcDiscountRate(roomPolicy, totalstaydays);

            //set order data: price/totprice/discount: 
            for (int i = 0; i < order.AccomodationOrderList.Count;i++ )
            {
                order.AccomodationOrderList[i] = this.CalcFinalRate(order.AccomodationOrderList[i], roomPolicy);
            }
            return order;
        }

        /// <summary>
        /// online 
        /// </summary>
        /// <param name="order"></param>
        private List<AccomodationPolicy> GetRoomPolicy(DateTime checkin, DateTime checkout, List<AccomodationDiscountPolicy> allDiscountRate)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" P.RoomPolicyID, P.StartDate, P.EndDate, P.RoomType,");
            sql.AppendLine(" P.WeekDays, P.BasicRate as BasicRateID, P.DiscountRate, R.BasicRate");
            sql.AppendLine(" FROM RoomPolicy P WITH(NOLOCK)");
            sql.AppendLine(" left join RoomRate R WITH(NOLOCK) on P.BasicRate = R.RoomRateID and R.IsDelete = 0");
            sql.AppendLine(" WHERE P.IsDelete = 0");
            sql.AppendLine(" and ((P.StartDate <= @CheckinDate and P.EndDate>=@CheckinDate)");
            sql.AppendLine(" or (P.StartDate <= @CheckoutDate and P.EndDate>=@CheckoutDate))");

            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("@CheckinDate", checkin);
            paras[1] = new SqlParameter("@CheckoutDate", checkout);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            List<AccomodationPolicy> accomodationPolicys = new List<AccomodationPolicy>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow rw in ds.Tables[0].Rows)
                {
                    AccomodationPolicy policy = new AccomodationPolicy();
                    policy.RoomType = Convert.ToInt32(rw["RoomType"]);
                    policy.StartDate = Convert.ToDateTime(rw["StartDate"]);
                    policy.EndDate = Convert.ToDateTime(rw["EndDate"]);
                    policy.WeekDays = rw["WeekDays"].ToString();

                    policy.BasicRateID = Convert.ToInt32(rw["BasicRateID"]);
                    policy.BasicRate = Convert.ToDouble(rw["BasicRate"]);

                    policy.DiscountPolicy = new List<AccomodationDiscountPolicy>();

                    List<AccomodationDiscountPolicy> lst = new List<AccomodationDiscountPolicy>();
                    if(!string.IsNullOrEmpty(rw["DiscountRate"].ToString()))
                    {
                        string[] ratesID = rw["DiscountRate"].ToString().Split(',');
                        foreach (string rateID in ratesID)
                        {
                            AccomodationDiscountPolicy discountPolicy = allDiscountRate.Find(m => m.RoomRateID.ToString() == rateID);
                            if (discountPolicy != null)
                            {
                                lst.Add(discountPolicy);
                            }
                        }

                        var a = from t in lst orderby t.DiscountDays descending select t;
                        policy.DiscountPolicy = a.ToList<AccomodationDiscountPolicy>();
                    }
                    accomodationPolicys.Add(policy);
                }

            }

            return accomodationPolicys;
        }

        private List<AccomodationDiscountPolicy> GetAllDiscountRate()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" RoomRateID, DiscountRate, DiscountDays");
            sql.AppendLine(" FROM RoomRate WITH(NOLOCK)");
            sql.AppendLine(" WHERE IsDelete = 0 and RateType = 1");
            sql.AppendLine(" order by DiscountDays desc");

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString());

            List<AccomodationDiscountPolicy> discountRate = new List<AccomodationDiscountPolicy>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                discountRate = DataMappingHelper<AccomodationDiscountPolicy>.DataTableToList(ds.Tables[0]);
            }

            return discountRate;
        }

        private List<AccomodationPolicy> CalcBasicRate(List<AccomodationPolicy> roomPolicy, string weekDays, int roomType)
        {
            AccomodationPolicy policy = roomPolicy.Find(m => m.WeekDays.Contains(weekDays.ToString()) && m.RoomType == roomType);
            policy.StayDays = policy.StayDays + 1;
            policy.Price = policy.StayDays * policy.BasicRate;
            //policy.SubTotal = policy.Price;

            return roomPolicy;

        }

        private List<AccomodationPolicy> CalcDiscountRate(List<AccomodationPolicy> roomPolicy, int totalstaydays)
        {
            foreach (AccomodationPolicy policy in roomPolicy)
            {
                int leftStayDays = policy.StayDays;
                double totalDiscount = 0;
                AccomodationDiscountPolicy discountPolicy1 = policy.DiscountPolicy[0];
                AccomodationDiscountPolicy discountPolicy2 = policy.DiscountPolicy[1];

                //foreach (AccomodationDiscountPolicy discountPolicy in policy.DiscountPolicy)
                //{
                    if (totalstaydays >= discountPolicy1.DiscountDays) 
                    {
                        totalDiscount = totalDiscount + discountPolicy1.DiscountRate * policy.BasicRate * policy.StayDays;
                       // leftStayDays = totalstaydays - policy.StayDays;
                    }
                  else if(totalstaydays <= discountPolicy2.DiscountDays)
                   {
                       totalDiscount = totalDiscount + discountPolicy2.DiscountRate * policy.BasicRate * policy.StayDays;
                   }
                //}
                policy.DiscountPrice = totalDiscount;
                policy.SubTotal = policy.Price - totalDiscount;

            }
            return roomPolicy;
        }

        private AccomodationOrderListModel CalcFinalRate(AccomodationOrderListModel order, List<AccomodationPolicy> roomPolicy)
        {
            List<AccomodationPolicy> lstPolicy = roomPolicy.FindAll(m => m.RoomType == order.RoomType);
            foreach (AccomodationPolicy policy in lstPolicy)
            {
                order.Price += policy.Price;
                order.DiscountPrice += policy.DiscountPrice;
                order.SubTotalPrice += policy.SubTotal;
            }
            return order;
        }

        #endregion

        public AccomodationOrderModel SaveOrder(AccomodationOrderModel order)
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
            str.AppendLine(" (OrderID, Name, Phone, Email, Address, State, PostCode, CreateTime, UpdateTime) ");
            str.AppendLine(" Values (@OrderID, @Name, @Phone, @Email, @Address, @State, @PostCode, Getdate(), getdate());");

            paras = new SqlParameter[7];
            paras[0] = new SqlParameter("@OrderID", order.OrderID);
            paras[1] = new SqlParameter("@Name", order.Contact.Name);
            paras[2] = new SqlParameter("@Phone", order.Contact.Phone);
            paras[3] = new SqlParameter("@Email", order.Contact.Email);
            paras[4] = new SqlParameter("@Address", order.Contact.Address);
            paras[5] = new SqlParameter("@State", order.Contact.State);
            paras[6] = new SqlParameter("@PostCode", order.Contact.PostCode);

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            #endregion 

            #region AccomodationOrderDetail
            order.TotalPrice = 0;
            double originalPrice = 0;
            double discount = 0;
            foreach (AccomodationOrderListModel detail in order.AccomodationOrderList)
            {
                if (detail.OrderRoomNumber > 0)
                {
                    detail.SubTotalPrice = (detail.Price - detail.DiscountPrice) * detail.OrderRoomNumber;
                    order.TotalPrice = order.TotalPrice + detail.SubTotalPrice;
                    originalPrice = originalPrice + detail.Price * detail.OrderRoomNumber;
                    discount = discount + detail.DiscountPrice * detail.OrderRoomNumber;

                    str = new StringBuilder();
                    str.AppendLine("Insert into RoomOrderDetail");
                    str.AppendLine(" (OrderID, RoomType, CheckinDate, CheckoutDate, Price, Discount, Quantity, TotalPrice, AdultCount, KidsCount, AdditionalDetail, CreateTime, UpdateTime) ");
                    str.AppendLine(" Values (@OrderID, @RoomType, @CheckinDate, @CheckoutDate, @Price, @Discount, @Quantity, @TotalPrice, @AdultCount, @KidsCount, @AdditionalDetail, getdate(), getdate());");

                    paras = new SqlParameter[11];
                    paras[0] = new SqlParameter("@OrderID", order.OrderID);
                    paras[1] = new SqlParameter("@RoomType", detail.RoomType);
                    paras[2] = new SqlParameter("@CheckinDate", Convert.ToDateTime(order.CheckInDate));
                    paras[3] = new SqlParameter("@CheckoutDate", Convert.ToDateTime(order.CheckOutDate));
                    paras[4] = new SqlParameter("@Price", detail.Price);
                    paras[5] = new SqlParameter("@Discount", detail.DiscountPrice);
                    paras[6] = new SqlParameter("@Quantity", detail.OrderRoomNumber);
                    
                    paras[7] = new SqlParameter("@TotalPrice", detail.SubTotalPrice);
                    paras[8] = new SqlParameter("@AdultCount", Convert.ToInt32(order.AccomodationOrderDetail.AdultCount));
                    paras[9] = new SqlParameter("@KidsCount", Convert.ToInt32(order.AccomodationOrderDetail.KidsCount));
                    paras[10] = new SqlParameter("@AdditionalDetail",  Common.VariableConvert.ConvertStringToDBValue(order.AccomodationOrderDetail.AdditionalDetail));

                    sqls.Add(str.ToString());
                    cmdParms.Add(paras);
                }


            }

            #endregion

            #region Order

            str = new StringBuilder();
            str.AppendLine("Insert into [Order]");
            str.AppendLine(" (OrderID, OrderType, OrderNo, ProcessType, OrderDate, OriginalPrice, Discount, Shipping, TotalPrice, Tax, OrderStatus, CreateTime, UpdateTime) ");
            str.AppendLine(" Values (@OrderID, @OrderType, @OrderNo, @ProcessType, @OrderDate, @OriginalPrice, @Discount, 0, @TotalPrice, @Tax, @OrderStatus, Getdate(), getdate());");

            paras = new SqlParameter[10];

            paras[0] = new SqlParameter("@OrderID", order.OrderID);
            int orderType = (int)GlobalVariable.OrderType.Room;
            paras[1] = new SqlParameter("@OrderType", orderType);
            order.OrderNo = "R" + order.OrderID.ToString().PadLeft(9, '0');
            paras[2] = new SqlParameter("@OrderNo", order.OrderNo);
            paras[3] = new SqlParameter("@ProcessType", order.ProcessType);
            order.OrderDate = DateTime.Now;
            paras[4] = new SqlParameter("@OrderDate", order.OrderDate);

            paras[5] = new SqlParameter("@OriginalPrice", originalPrice);
            paras[6] = new SqlParameter("@Discount", discount);

            paras[7] = new SqlParameter("@TotalPrice", order.TotalPrice);

            double tax = order.TotalPrice * Convert.ToDouble(ConfigurationManager.AppSettings["TaxRate"].ToString());
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

        public AccomodationOrderModel SaveOrderResult(AccomodationOrderModel order)
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
            paras[1] = new SqlParameter("@OrderDesc", Common.VariableConvert.ConvertStringToDBValue(order.ResultMsg));
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
