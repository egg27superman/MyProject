using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AFH.Barcaldine.Models
{
    /// <summary>
    /// offline: get the wine products
    /// </summary>
    public class ProductListModel
    {
        public ProductSearchModel ProductSearch { get; set; }
        public List<ProductResultModel> ProductResult { get;set; }
    }

    /// <summary>
    /// offline: search conditions
    /// </summary>
    public class ProductSearchModel
    {
        public string IsDelete { get; set; }

        public int CategoryID { get; set; }

        public IEnumerable<SelectListItem> GetWineCategoryList { get; set; }

        public IEnumerable<SelectListItem> GetIsDeleteSelectList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "9", Selected = true });
            items.Add(new SelectListItem { Text = "No Delete", Value = "0" });
            items.Add(new SelectListItem { Text = "Delete", Value = "1" });

            return items;
        }

    }

    /// <summary>
    /// offline: search result
    /// </summary>
    public class ProductResultModel
    {
        public int WineCategoryID { get; set; }
        public string WineCategoryName { get; set; }

        public string WineID { get; set; }
        public string Describle { get; set; }

        public double Price { get; set; }
        public int ProductYear { get; set; }

        public bool IsDelete { get; set; }
    }
}
