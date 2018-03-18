using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace AFH.Barcaldine.Models
{
    public class FaultOrderListModel
    {
        public FaultOrderListSearchModel FaultOrderListSearch { get; set; }
        public List<FaultOrderListResultModel> FaultOrderListResult { get; set; }

    }

    public class FaultOrderListSearchModel : OrderListSearchModel
    {

        public int IsDelete { get; set; }

        public IEnumerable<SelectListItem> GetIsDeleteSelectList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "9", Selected = true });
            items.Add(new SelectListItem { Text = "No Delete", Value = "0" });
            items.Add(new SelectListItem { Text = "Delete", Value = "1" });

            return items;
        }

    }

    public class FaultOrderListResultModel : OrderListResultModel
    {
        public string FaultMessage { get; set; }
        public bool IsDelete { get; set; }
    }
}
