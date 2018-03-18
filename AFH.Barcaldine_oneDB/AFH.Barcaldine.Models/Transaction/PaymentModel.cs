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
    public class PaymentModel
    {

        public string creditCard1 { get; set; }

        public string creditCard2 { get; set; }

        public string creditCard3 { get; set; }
        public string creditCard4 { get; set; }

        public string ExpiryYear { get; set; }
        public IEnumerable<SelectListItem> GetYear()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Year", Value = "Y", Selected = true });
            for (int i = 0; i < 10; i++)
            {
                items.Add(new SelectListItem { Text = (DateTime.Now.Year + i).ToString(), Value = (DateTime.Now.Year + i).ToString().Substring(2,2) });
            }
            return items;
        }

        public string ExpiryMonth { get; set; }
        public IEnumerable<SelectListItem> GetMonth()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Month", Value = "M", Selected = true });
            items.Add(new SelectListItem { Text = "Jan", Value = "01" });
            items.Add(new SelectListItem { Text = "Feb", Value = "02"});
            items.Add(new SelectListItem { Text = "Mar", Value = "03" });
            items.Add(new SelectListItem { Text = "Apr", Value = "04" });
            items.Add(new SelectListItem { Text = "May", Value = "05" });
            items.Add(new SelectListItem { Text = "Jun", Value = "06" });
            items.Add(new SelectListItem { Text = "Jul", Value = "07" });
            items.Add(new SelectListItem { Text = "Aug", Value = "08" });
            items.Add(new SelectListItem { Text = "Sep", Value = "09" });
            items.Add(new SelectListItem { Text = "Oct", Value = "10" });
            items.Add(new SelectListItem { Text = "Nov", Value = "11" });
            items.Add(new SelectListItem { Text = "Dec", Value = "12" });

            return items;
        }

        public string CVV { get; set; }

    }
}
