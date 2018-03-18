using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AFH.Barcaldine.Models
{
    public class RoomRateListModel
    {
        public RoomRateSearchModel RoomRateSearch { get; set; }
        public List<RoomRateResultModel> RoomRateResult { get; set; }
    }

    public class RoomRateSearchModel
    {
        public string RateName { get; set; }

        public int RateType { get; set; }

        public IEnumerable<SelectListItem> GetRateTypeList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "9", Selected = true });
            items.Add(new SelectListItem { Text = "Basic Rate", Value = "0" });
            items.Add(new SelectListItem { Text = "Discount Rate", Value = "1" });

            return items;
        }

        public string IsDelete { get; set; }

        public IEnumerable<SelectListItem> GetIsDeleteSelectList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "9", Selected = true });
            items.Add(new SelectListItem { Text = "No Delete", Value = "0" });
            items.Add(new SelectListItem { Text = "Delete", Value = "1" });

            return items;
        }
    }

    public class RoomRateResultModel
    {
        public int RoomRateID { get; set; }
        public string RateName { get; set; }
        public int RateType { get; set; }
        public string RateTypeName { get; set; }
        public bool IsDelete { get; set; }
    }
}
