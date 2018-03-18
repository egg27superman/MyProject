using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AFH.Barcaldine.Models
{
    public class WineCategoryListModel
    {
        public WineCategorySearchModel WineCategorySearch { get; set; }

        public List<WineCategoryResultModel> WineCategoryResult { get; set; }
        
    }

    public class WineCategorySearchModel
    {
        public string IsDelete { get; set; }

        public IEnumerable<SelectListItem> GetSelectList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "9", Selected = true });
            items.Add(new SelectListItem { Text = "No Delete", Value = "0" });
            items.Add(new SelectListItem { Text = "Delete", Value = "1" });

            return items;
        }
    }

    public class WineCategoryResultModel
    {
        public int WineCategoryID { get; set; }
        public string WineCategoryName { get; set; }
        public bool IsDelete { get; set; }
    }
}
