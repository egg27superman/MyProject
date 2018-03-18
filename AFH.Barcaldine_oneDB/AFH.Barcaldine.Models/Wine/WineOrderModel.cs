using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;



namespace AFH.Barcaldine.Models
{
    public class WineOrderModel
    {
        public List<WineListModel> Wines { get; set; }

        public DeliveryModel Delivery { get; set; }
        public double Shipping { get; set; }

        public PaymentModel Payment { get; set; }

        public string ShippingInfo { get; set; }
        
        public int OrderID { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public double OriginalPrice { get; set; }
        public double TotalOrderPrice { get; set; }
        public int ResultStatus { get; set; }
        public string ResultMsg { get; set; }
        public string RequestID { get; set; }

        public PaymentResponseModel Response { get; set; }

        public int ProcessType { get; set; }

    }

    public class WineListModel
    {
        public int WineID { get; set; }
        public string WineName { get; set; }
        public int ProductYear { get; set; }
        public double Price { get; set; }

        public int Bottle { get; set; }
        public int TotalPrice { get; set; }
    }

    public class DeliveryModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime? Birthday { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string PostCode { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [RegularExpression(@"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$", ErrorMessage = "Email is invalid.")]
        public string Email { get; set; }

        [Required]
        public string State { get; set; }

        public IEnumerable<SelectListItem> GetStateList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            
            items.Add(new SelectListItem { Text = "NSW", Value = "NSW" });
            items.Add(new SelectListItem { Text = "ACT", Value = "ACT" });
            items.Add(new SelectListItem { Text = "VIC", Value = "VIC", Selected = true });
            items.Add(new SelectListItem { Text = "QLD", Value = "QLD" });
            items.Add(new SelectListItem { Text = "SA", Value = "SA" });
            items.Add(new SelectListItem { Text = "WA", Value = "WA" });
            items.Add(new SelectListItem { Text = "TAS", Value = "TAS" });
            items.Add(new SelectListItem { Text = "NT", Value = "NT" });

            return items;
        }
    }

    public class WineOrderDetail
    {
        public int WineID { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        //public double Discount { get; set; }
        //public double TotalPrice { get; set; }
    }

    public class ShippingInfoModel
    {
        public string State { get; set; }
        public double ShippingRate { get; set; }
    }

}
