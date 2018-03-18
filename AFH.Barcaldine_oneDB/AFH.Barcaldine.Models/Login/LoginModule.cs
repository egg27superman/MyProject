using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;



namespace AFH.Barcaldine.Models
{
   public class LoginModule
    {
        //public int ID { get; set; }


        [Required]
        [Display(Name = "User Name")]
        [StringLength(10)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string PassWord { get; set; }


        [Display(Name = "Sign In")]
        public string SignIn { get; set; }

         
    }
}
