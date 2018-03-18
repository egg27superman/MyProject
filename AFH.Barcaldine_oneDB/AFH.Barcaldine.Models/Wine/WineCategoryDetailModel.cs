using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AFH.Barcaldine.Models
{
    public class WineCategoryDetailModel
    {
        public int WineCategoryID { get; set; }

        //public string WineCategoryName { get; set; }

        public bool IsDelete { get; set; }

        public WineCategoryNameModel WineCategoryName { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public string OpertationStatus { get; set; }


        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }

    }


}
