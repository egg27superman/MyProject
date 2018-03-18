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
    public class RoomStatusController : Controller
    {
        //
        // GET: /RoomStatus/


        #region Search

        public ActionResult RoomStatus()
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            RoomStatusModel model = new RoomStatusModel();

            try
            {
                //Default data: Select all data
                RoomStatusCore RoomStatusCore = new RoomStatusCore();
                model.Year = DateTime.Now.Year;
                model.Month = DateTime.Now.Month;
                model.RoomType = GlobalVariable.RoomType.King;

                model = RoomStatusCore.GetSearchData(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Room Rate list (httpget) exception. ex:" + ex.Message);
                _logHelper.WriteLog("Room Rate list (httpget) exception.", ex.Message, string.Empty);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult RoomStatus(RoomStatusModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            try
            {

                RoomStatusCore RoomStatusCore = new RoomStatusCore();
                model = RoomStatusCore.GetSearchData(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Room state (httppost) exception:" + ex.Message);
                _logHelper.WriteLog("Room state (httppost) exception.", ex.Message, string.Empty);
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult RoomStatusUpdate(RoomStatusModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            try
            {

                RoomStatusCore roomStatusCore = new RoomStatusCore();
                if (model.ChooseDate == null) model.ChooseDate = string.Empty;
                model.UpdateUser = LoginVariable.UserName;
                roomStatusCore.UpdateData(model);


                model.ResultStatus = (int)GlobalVariable.OrderStatus.Success;
                model.ResultMsg = "Save data Success";
            }
            catch (Exception ex)
            {
                model.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                model.ResultMsg = "RoomStatusUpdate (HttpPost) exception:" + ex.Message;

                ModelState.AddModelError("", "RoomStatusUpdate (HttpPost) exception:" + ex.Message);
                _logHelper.WriteLog("RoomStatusUpdate (HttpPost) exception.", ex.Message, string.Empty);
            }

            return View("RoomStatus", model);
        }



        #endregion


    }
}
