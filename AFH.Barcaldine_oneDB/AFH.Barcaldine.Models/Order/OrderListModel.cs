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
    public class OrderListModel
    {
        public OrderListSearchModel OrderListSearch { get; set; }
        public List<OrderListResultModel> OrderListResult { get; set; }
    }

    public class OrderListSearchModel
    {
        public string OrderNo { get; set; }

        public int OrderType { get; set; }
        public IEnumerable<SelectListItem> GetOrderTypeList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "9", Selected = true });
            items.Add(new SelectListItem { Text = "Room", Value = "0" });
            items.Add(new SelectListItem { Text = "Wine", Value = "1" });
            return items;
        }

        public int ProcessType { get; set; }
        public IEnumerable<SelectListItem> GetProcessTypeList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "9", Selected = true });
            items.Add(new SelectListItem { Text = "Online", Value = "0" });
            items.Add(new SelectListItem { Text = "Offline", Value = "1" });
            return items;
        }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? OrderDateStart { get; set; }
        public DateTime? OrderDateEnd { get; set; }

        public int OrderStatus { get; set; }
        public IEnumerable<SelectListItem> GetOrderStatusList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "9", Selected = true });
            items.Add(new SelectListItem { Text = "Success", Value = "1" });
            items.Add(new SelectListItem { Text = "Fail", Value = "2" });
            items.Add(new SelectListItem { Text = "Exception", Value = "3" });
            return items;
        }
    }

    public class OrderListResultModel
    {
        public string OrderID { get; set; }
        public string OrderNo { get; set; }

        public string OrderTypeName { get; set; }
        public string ProcessTypeName { get; set; }

        public DateTime OrderDate { get; set; }

        public double OriginalPrice { get; set; }
        public double Discount { get; set; }
        public double Shipping { get; set; }
        public double TotalPrice { get; set; }
        public double Tax { get; set; }

        public string OrderStatusName { get; set; }
        public string OrderDesc { get; set; }

    }
}

