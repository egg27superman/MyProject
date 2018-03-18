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
    public class ShippingDetailModel
    {
        public int ShippingID { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [Range(1, 1000000)]
        public double? ShippingRate { get; set; }

        public bool IsDelete { get; set; }

        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }

        public string OpertationStatus { get; set; }


        public IEnumerable<SelectListItem> GetStateList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //items.Add(new SelectListItem { Text = "All", Value = "9", Selected = true });
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

    }
}
