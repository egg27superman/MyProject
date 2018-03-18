using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AFH.Barcaldine.Models
{
    public class RoomStatusModel
    {
        public int Year { get; set; }

        public IEnumerable<SelectListItem> GetYearList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 2014; i < 2024; i++)
            {
                items.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            return items;
        }

        public int Month { get; set; }
        public IEnumerable<SelectListItem> GetMonthList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "January", Value = "1" });
            items.Add(new SelectListItem { Text = "February", Value = "2" });
            items.Add(new SelectListItem { Text = "March", Value = "3" });
            items.Add(new SelectListItem { Text = "April", Value = "4" });
            items.Add(new SelectListItem { Text = "May", Value = "5" });
            items.Add(new SelectListItem { Text = "June", Value = "6" });
            items.Add(new SelectListItem { Text = "July", Value = "7" });
            items.Add(new SelectListItem { Text = "August", Value = "8" });
            items.Add(new SelectListItem { Text = "September", Value = "9" });
            items.Add(new SelectListItem { Text = "Octorber", Value = "10" });
            items.Add(new SelectListItem { Text = "November", Value = "11" });
            items.Add(new SelectListItem { Text = "December", Value = "12" });

            return items;
        }

        public int RoomType { get; set; }



        public string ChooseDate { get; set; }
        public string OpertationStatus { get; set; }

        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }

        public int ResultStatus { get; set; }
        public string ResultMsg { get; set; }
    }

}
