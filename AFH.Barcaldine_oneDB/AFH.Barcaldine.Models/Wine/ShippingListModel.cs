using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AFH.Barcaldine.Models
{
    public class ShippingListModel
    {
        public ShippingSearchModel ShippingSearch { get; set; }

        public List<ShippingResultModel> ShippingResult { get; set; }
    }

    public class ShippingSearchModel
    {
        public string IsDelete { get; set; }

        public string State { get; set; }

        public IEnumerable<SelectListItem> GetStateList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "9", Selected = true });
            items.Add(new SelectListItem { Text = "NSW", Value = "NSW" });
            items.Add(new SelectListItem { Text = "ACT", Value = "ACT" });
            items.Add(new SelectListItem { Text = "VIC", Value = "VIC" });
            items.Add(new SelectListItem { Text = "QLD", Value = "QLD" });
            items.Add(new SelectListItem { Text = "SA", Value = "SA" });
            items.Add(new SelectListItem { Text = "WA", Value = "WA" });
            items.Add(new SelectListItem { Text = "TAS", Value = "TAS" });
            items.Add(new SelectListItem { Text = "NT", Value = "NT" });

            return items;
        }

        public IEnumerable<SelectListItem> GetIsDeleteSelectList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "9", Selected = true });
            items.Add(new SelectListItem { Text = "No Delete", Value = "0" });
            items.Add(new SelectListItem { Text = "Delete", Value = "1" });

            return items;
        }
    }

    public class ShippingResultModel
    {
        public int ShippingID { get; set; }

        public string State { get; set; }

        public double ShippingRate { get; set; }

        public bool IsDelete { get; set; }
    }

}
