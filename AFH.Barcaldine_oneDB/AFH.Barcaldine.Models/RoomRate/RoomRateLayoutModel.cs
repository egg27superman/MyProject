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
    public class RoomRateLayoutModel
    {
        public List<PrivilegeDetailModel> UserPrivilegeLayout { get; set; }
    }
}
