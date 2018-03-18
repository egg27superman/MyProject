using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace AFH.Barcaldine.Models
{
    public class ChangePasswordModule
    {
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Current Password")]
        [StringLength(20, MinimumLength = 6)]
        public string CurrentPassword { get; set; }


        [Required]
        [Display(Name = "New Password")]
        [StringLength(20, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [StringLength(20, MinimumLength = 6)]
        public string ConfirmPassword { get; set; }


        public DateTime UpdateTime { get; set; }

        public string UpdateUser { get; set; }
    }
}
