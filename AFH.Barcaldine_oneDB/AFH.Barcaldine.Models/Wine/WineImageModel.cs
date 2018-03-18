using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFH.Barcaldine.Models
{
    public class WineImageModel
    {
        public int WineImageID { get; set; }
        public int WineID { get; set; }
        public string Url { get; set; }
        public int ImageType { get; set; }

    }
}
