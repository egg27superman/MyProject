using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFH.Barcaldine.Models
{
    /// <summary>
    /// online: get wine products
    /// </summary>
    public class ProductView
    {
        public int WineID { get; set; }
        
        public int WineCategoryID { get; set; }
        public WineCategoryNameModel WineCategoryName { get; set; }

        public int ProductYear { get; set; }

        public double Price { get; set; }

        public List<ProductDescribleModel> ProductDescrible { get; set; }

        public string Region { get; set; }

        public string ListPicture { get; set; }
        public string DetailPicture { get; set; }

    }
}
