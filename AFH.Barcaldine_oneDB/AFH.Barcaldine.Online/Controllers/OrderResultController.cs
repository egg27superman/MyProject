using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using AFH.Barcaldine.Models;
using AFH.Barcaldine.Log;
using AFH.Barcaldine.Common;

namespace AFH.Barcaldine.Online.Controllers
{
    public class OrderResultController : Controller
    {
        //
        // GET: /OrderResult/

        public ActionResult Success(OrderSuccessModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Online, this.ControllerContext.RouteData.Values["action"].ToString());

            return View(model);
        }

    }
}
