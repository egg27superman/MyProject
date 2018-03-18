using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;


namespace AFH.Barcaldine.Models
{
    public class AccomodationOrderModel
    {
        [Required]
        public DateTime? CheckInDate { get; set; }
        [Required]
        public DateTime? CheckOutDate { get; set; }

        public List<AccomodationOrderListModel> AccomodationOrderList { get; set; }

        public double TotalPrice { get; set; }

        public AccomodationOrderDetailModel AccomodationOrderDetail { get; set; }

        public ContactModel Contact { get; set; }

        public PaymentModel Payment { get; set; }


        public int OrderID { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }

        public string RequestID { get; set; }
        public PaymentResponseModel Response { get; set; }

        public int ResultStatus { get; set; }
        public string ResultMsg { get; set; }

        public int ProcessType { get; set; }
    }

    public class AccomodationOrderListModel
    {
        public int RoomType { get; set; }
        public int AvailableRoomNumber { get; set; }
        public int OrderRoomNumber { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public double SubTotalPrice { get; set; }

        public IEnumerable<SelectListItem> GetAvailableRoomList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 0; i <= AvailableRoomNumber; i++)
            {
                items.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            return items;
        }

    }

    public class AccomodationPolicy
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RoomType { get; set; }

        public string WeekDays { get; set; }


        public int BasicRateID { get; set; }
        public double BasicRate { get; set; }
        public double Price { get; set; }

        public List<AccomodationDiscountPolicy> DiscountPolicy { get; set; }
        public double DiscountPrice { get; set; }

        public int StayDays { get; set; }
        public double SubTotal { get; set; }

    }

    public class AvailableRoom
    {
        public int RoomNumer { get; set; }
        public DateTime TimeKey { get; set; }

    }

    public class AccomodationDiscountPolicy
    {
        public int RoomRateID { get; set; }
        public double DiscountRate { get; set; }
        public int DiscountDays { get; set; }
    }

    public class ContactModel
    {
        [Required]
        public string Name { get; set; }

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

    public class AccomodationOrderDetailModel
    {
        [Required]
        public int? AdultCount { get; set; }
        [Required]
        public int? KidsCount { get; set; }
        public string AdditionalDetail { get; set; }

    }
}
