using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

using AFH.Barcaldine.Models;
using AFH.Barcaldine.Core;
using AFH.Barcaldine.Common;
using AFH.Barcaldine.Log;

namespace AFH.Barcaldine.Offline.Controllers
{
    public class MenuController : Controller
    {
        //
        // GET: /Menu/

        //[ChildActionOnly]
        public ActionResult Menu()
        {
            MenuCore core = new MenuCore();
            MenuModule menus = core.GetMenu(LoginVariable.UserID);
            return PartialView("_PartialMenu", menus);
        }

    }
}
