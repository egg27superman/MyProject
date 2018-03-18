using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFH.Barcaldine.Models
{
    /// <summary>
    /// Online: Wine Detail page
    /// </summary>
    public class WineDetailModel
    {

        public int WineID { get; set; }

        public int WineCategoryID { get; set; }
        public WineCategoryNameModel WineCategoryName { get; set; }

        public int ProductYear { get; set; }
        
        public double Price { get; set; }

        public ProductDescribleModel Describle { get; set; }

        public string Region { get; set; }

        public string DetailPicture { get; set; }

        public List<WineInfo> Wines { get; set; }

        public List<WineCategoryDetailModel> Categorys { get; set; }


    }

    public class WineInfo
    {
        public int WineID { get; set; }
        public string WineName { get; set; }

        public int WineCategoryID { get; set; }
    }
}
