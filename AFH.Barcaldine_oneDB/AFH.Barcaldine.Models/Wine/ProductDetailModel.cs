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
    /// <summary>
    /// offline: insert/update the wine product
    /// </summary>
    public class ProductDetailModel
    {
        public int WineID { get; set; }

        
        public int WineCategoryID { get; set; }

        [DisplayName("Year")]
        [Required]
        [Range(1900, 2100)]
        public int? ProductYear { get; set; }

        [Required]
        [Range(1, 1000000)]
        public double? Price { get; set; }

        public List<ProductDescribleModel> Describle { get; set; }

        public string Region { get; set; }

        public bool IsDelete { get; set; }

        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }

        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }

        public string OpertationStatus { get; set; }
        public IEnumerable<SelectListItem> GetWineCategoryList { get; set; }

        //public string ListImage { get; set; }
        //public string DetailImage { get; set; }

        public List<WineImageModel> WineImages { get; set; }
    }
}
