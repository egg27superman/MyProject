using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFH.Barcaldine.Models
{
    public class OrderDetailModel
    {
        public int OrderID { get; set; }
        public string SourceFrom { get; set; }

        public string OrderNo { get; set; }

        public int OrderType { get; set; }
        public string OrderTypeName { get; set; }
        public int ProcessType { get; set; }
        public string ProcessTypeName { get; set; }

        public DateTime OrderDate { get; set; }

        public double OriginalPrice { get; set; }
        public double Discount { get; set; }
        public double Shipping { get; set; }
        public double TotalPrice { get; set; }
        public double Tax { get; set; }

        public int OrderStatus { get; set; }
        public string OrderStatusName { get; set; }
        public string OrderDesc { get; set; }

        public List<WineOrderDetailModel> WineOrderDetail { get; set; }
        public List<RoomOrderDetailModel> RoomOrderDetail { get; set; }

        public CustomerModel Customer { get; set; }
        public PaymentTransactionModel PaymentTransaction { get; set; }
        public List<LogModel> Log { get; set; }
        public FaultOrderModel FaultOrder { get; set; }

    }

    public class WineOrderDetailModel
    {
        public int WineID { get; set; }
        public string WineName { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }
        public double Discount { get; set; }
        public double TotalPrice { get; set; }

    }

    public class RoomOrderDetailModel
    {
        public string RoomType { get; set; }
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }

        public double Price { get; set; }
        public double Discount { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }

        public int AdultCount { get; set; }
        public int KidsCount { get; set; }
        public string AdditionalDetail { get; set; }
    }

    public class CustomerModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
    }

    public class PaymentTransactionModel
    {
        public string RequestID { get; set; }
        public string ResponseID { get; set; }
        public string ResponseStatus { get; set; }
        public string ResponseMsg { get; set; }
    }

    public class LogModel
    {
        public string LogContent { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime LogTime { get; set; }
        public string System { get; set; }
        public string SystemModel { get; set; }
    }

    public class FaultOrderModel
    {
        public string FaultMessage { get; set; }
        public bool IsDelete { get; set; }
    }
}
