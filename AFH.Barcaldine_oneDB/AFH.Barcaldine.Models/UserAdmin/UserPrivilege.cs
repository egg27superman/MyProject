using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.Web.Mvc;

namespace AFH.Barcaldine.Models.UserAdmin
{
    public class UserPrivilege
    {
        public int  PrivilegeID { get; set; }

        [Display(Name = "Room Basic Rate")]
        public bool RoomBasicRate { get; set; }

        [Display(Name = "Room Rate Policy")]
        public bool RoomRatePolicy { get; set; }

        [Display(Name = "Room Status")]
        public bool RoomStatus { get; set; }

        [Display(Name = "Wine Category")]
        public bool WineCategory { get; set; }

        [Display(Name = "Wine Product")]
        public bool WineProduct { get; set; }

        [Display(Name = "Shipping")]
        public bool Shipping { get; set; }

       
    }
}
