using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace AFH.Barcaldine.Models
{
    public class RatePolicyDetailModel
    {
        public int RoomPolicyID { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }
        
        [Required]
        public DateTime? EndDate { get; set; }

        [Required]
        public int? RoomType { get; set; }
        
        [Required]
        public string WeekDays { get; set; }
        
        [Required]
        public int? BasicRate { get; set; }
        public List<RoomRateDetailModel> BasicRateList { get; set; }

        [Required]
        public string DiscountRate { get; set; }
        public List<RoomRateDetailModel> DiscountRateList { get; set; }

        public bool IsDelete { get; set; }

        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }

        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }

        public string OpertationStatus { get; set; }
    }

}
