using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AFH.Barcaldine.Models
{
    public class RatePolicyListModel
    {
        public RatePolicySearchModel RatePolicySearch { get; set; }
        public List<RatePolicyResultModel> RatePolicyResult { get; set; }
    }

    public class RatePolicySearchModel
    {
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }

        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }

        public int RoomType { get; set; }

        public IEnumerable<SelectListItem> GetRoomTypeList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "9", Selected = true });
            items.Add(new SelectListItem { Text = "King", Value = "0" });
            items.Add(new SelectListItem { Text = "Queen", Value = "1" });

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


    public class RatePolicyResultModel
    {
        public int RoomPolicyID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int RoomType { get; set; }
        public string RoomTypeName { get; set; }

        public string WeekDays { get; set; }
        public string BasicRate { get; set; }
        public string DiscountRate { get; set; }

        public bool IsDelete { get; set; }

    }
}
