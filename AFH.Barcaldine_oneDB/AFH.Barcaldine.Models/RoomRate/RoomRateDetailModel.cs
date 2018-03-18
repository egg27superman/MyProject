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
    public class RoomRateDetailModel
    {
        public int RoomRateID { get; set; }
        public string RateName { get; set; }

        public int RateType { get; set; }

        [Range(0, 2000000)]
        public double? BasicRate { get; set; }
        [Range(0, 100)]
        public double? DiscountRate { get; set; }
        [Range(0, 365)]
        public int? DiscountDays { get; set; }


        public bool IsDelete { get; set; }


        public string OpertationStatus { get; set; }


        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
