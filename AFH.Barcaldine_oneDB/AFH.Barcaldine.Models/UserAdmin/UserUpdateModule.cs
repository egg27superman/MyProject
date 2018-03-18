using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.Web.Mvc;

namespace AFH.Barcaldine.Models
{
    public class UserUpdateModule
    {
        public int UserID { get; set; }

//        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

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

        [Display(Name = "Is Delete")]
        public bool IsDelete { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        public string OpertationStatus { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUser { get; set; }

        public DateTime UpdateTime { get; set; }

        public string UpdateUser { get; set; }

        public List<PrivilegeDetailModel> UserPrivilegeList { get; set; }

        public UserResultSearchModule UserNameSearch { get; set; }
    }

    public class PrivilegeDetailModel
    {
        public int MenuID { get; set; }

        public string menuname { get; set; }

        public int UserID { get; set; }

        public bool IsSelected { get; set;}
    }

    public class UserListModule1
    {
        public UserSearchModule UserSearch { get; set; }
        public List<UserResultListModule> UserResultList { get; set; }
    }

    public class UserSearchModule1
    {
        public int? UserID { get; set; }
        public string UserName { get; set; }

        public int IsDelete { get; set; }

        public IEnumerable<SelectListItem> GetSelectList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "0", Selected = true });
            items.Add(new SelectListItem { Text = "Delete", Value = "1" });
            items.Add(new SelectListItem { Text = "No Delete", Value = "2" });

            return items;
        }
    }

    public class UserResultSearchModule
    {
        public int UserID { get; set; }
       public IEnumerable<SelectListItem> GetUserNameList { get; set; }



  
    }
}
