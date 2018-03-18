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
    public class UserListModule
    {
        public UserSearchModule UserSearch { get; set; }
        public List<UserResultListModule> UserResultList { get; set; }
    }

    public class UserSearchModule
    {
        public int? UserID { get; set; }
        public string UserName { get; set; }
    }

    public class UserResultListModule
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public bool IsDelete { get; set; }
    }
}
