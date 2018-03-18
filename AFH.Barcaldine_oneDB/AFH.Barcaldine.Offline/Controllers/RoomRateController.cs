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
    public class RoomRateController : Controller
    {
        //
        // GET: /RoomPrice/

        #region Room Rate

        public ActionResult RoomRateList()
        {
          
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            RoomRateListModel model = new RoomRateListModel();
            try
            {
                //Default data: Select all data
                RoomRateCore RoomRateCore = new RoomRateCore();
                model.RoomRateSearch = new RoomRateSearchModel();
                model.RoomRateSearch.IsDelete = "9";
                model.RoomRateSearch.RateType = 9;
                model.RoomRateResult = RoomRateCore.GetSearchData(model.RoomRateSearch);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Room Rate list (httpget) exception. ex:" + ex.Message);
                _logHelper.WriteLog("Room Rate list (httpget) exception.", ex.Message, string.Empty);
            }
           
            return View(model);
        }

        [HttpPost]
        public ActionResult RoomRateList(RoomRateListModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            try
            {

                RoomRateCore roomRateCore = new RoomRateCore();
                model.RoomRateResult = roomRateCore.GetSearchData(model.RoomRateSearch);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Room Rate List (httpget) exception:" + ex.Message);
                _logHelper.WriteLog("Room Rate List (httpget) exception.", ex.Message, string.Empty);
            }

            return View(model);
        }

        public ActionResult RoomRateDetail(string opertation, string id)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            RoomRateDetailModel roomRate = new RoomRateDetailModel();
            try
            {
                if (opertation == GlobalVariable.UpdateStatus.Add)
                {
                    //add
                    roomRate.OpertationStatus = opertation;
                }
                else if (opertation == GlobalVariable.UpdateStatus.Update)
                {
                    //update
                    RoomRateCore roomRateCore = new RoomRateCore();
                    roomRate = roomRateCore.GetDataByID(id);
                    roomRate.OpertationStatus = opertation;
                    roomRate.DiscountRate = roomRate.DiscountRate * 100;

                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Room rate Detail(httpget) exception:" + ex.Message);
                _logHelper.WriteLog("Room rate Detail(httpget) exception. ", ex.Message, string.Empty);
            }
            return View(roomRate);
        }

        [HttpPost]
        public ActionResult RoomRateDetail(RoomRateDetailModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.RateType == 0)
                    {
                        if (model.BasicRate <= 0)
                        {
                            ModelState.AddModelError("", "Basic Rate must be higher than 0");
                            return View(model);
                        }
                        model.DiscountRate = 0;
                        model.DiscountDays = 0;
                    }
                    else if (model.RateType == 1)
                    {
                        if (model.DiscountDays <= 0)
                        {
                            ModelState.AddModelError("", "Discount Days must be higher than 0");
                            return View(model);
                        }
                        if (model.DiscountRate <= 0)
                        {
                            ModelState.AddModelError("", "Discount Rate must be higher than 0");
                            return View(model);
                        }
                        model.BasicRate = 0;
                        model.DiscountRate = model.DiscountRate / 100;
                    }

                    RoomRateCore roomRate = new RoomRateCore();
                    if (model.OpertationStatus == GlobalVariable.UpdateStatus.Add)
                    {
                        //insert new room rate

                        model.CreateUser = LoginVariable.UserName;
                        model.UpdateUser = LoginVariable.UserName;
                        roomRate.InsertData(model);
                        _logHelper.WriteLog("add room rate. username:" + model.CreateUser);
                        return RedirectToAction("RoomRateList", "RoomRate");

                    }
                    else if (model.OpertationStatus == GlobalVariable.UpdateStatus.Update)
                    {
                        //update room rate
                        model.UpdateUser = LoginVariable.UserName;
                        roomRate.UpdateData(model);

                        _logHelper.WriteLog("update room rate success. username:" + model.UpdateUser);

                        return RedirectToAction("RoomRateList", "RoomRate");

                    }
                }

            }
            catch (Exception ex)
            {
                _logHelper.WriteLog("RoomRateDetail(httppost) exception. operator=" + model.OpertationStatus, ex.Message, string.Empty);
            }
            return View(model);
        }

        #endregion

        #region Rate Policy

        public ActionResult RatePolicyList()
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            RatePolicyListModel model = new RatePolicyListModel();
            try
            {
                //Default data: Select all data
                RoomRatePolicyCore RoomRateCore = new RoomRatePolicyCore();
                model.RatePolicySearch = new RatePolicySearchModel();
                model.RatePolicySearch.IsDelete = "9";
                model.RatePolicySearch.RoomType = 9;
                model.RatePolicyResult = RoomRateCore.GetSearchData(model.RatePolicySearch);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Room Policy list (httpget) exception. ex:" + ex.Message);
                _logHelper.WriteLog("Room Policy list (httpget) exception.", ex.Message, string.Empty);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult RatePolicyList(RatePolicyListModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            try
            {

                RoomRatePolicyCore roomRatePolicyCore = new RoomRatePolicyCore();
                model.RatePolicyResult = roomRatePolicyCore.GetSearchData(model.RatePolicySearch);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Room Policy List (httpget) exception:" + ex.Message);
                _logHelper.WriteLog("Room Policy List (httpget) exception.", ex.Message, string.Empty);
            }

            return View(model);
        }

        public ActionResult RatePolicyDetail(string opertation, string id)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            RatePolicyDetailModel roomRatePolicy = new RatePolicyDetailModel();
            
            RoomRateCore roomRateCore = new RoomRateCore();
            RoomRatePolicyCore roomRatePolicyCore = new RoomRatePolicyCore();

            try
            {
                if (opertation == GlobalVariable.UpdateStatus.Add)
                {
                    //add
                    roomRatePolicy.OpertationStatus = opertation;
                }
                else if (opertation == GlobalVariable.UpdateStatus.Update)
                {
                    //update
                    roomRatePolicy = roomRatePolicyCore.GetDataByID(id);
                    roomRatePolicy.OpertationStatus = opertation;
                    
                }

                //basic rate list init
                roomRatePolicy.BasicRateList = roomRateCore.GetDataByRateType(GlobalVariable.RoomRateType.Basic);

                //discount rate list init
                roomRatePolicy.DiscountRateList = roomRateCore.GetDataByRateType(GlobalVariable.RoomRateType.Discount);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Room rate policy detail(httpget) exception:" + ex.Message);
                _logHelper.WriteLog("Room rate policy detail(httpget) exception. ", ex.Message, string.Empty);
            }
            return View(roomRatePolicy);
        }

        [HttpPost]
        public ActionResult RatePolicyDetail(RatePolicyDetailModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());
            RoomRateCore roomRateCore = new RoomRateCore();
            RoomRatePolicyCore roomRatePolicy = new RoomRatePolicyCore();

            try
            {
                if (ModelState.IsValid)
                {
                    if (model.EndDate < model.StartDate)
                    {
                        //basic rate list init
                        model.BasicRateList = roomRateCore.GetDataByRateType(GlobalVariable.RoomRateType.Basic);

                        //discount rate list init
                        model.DiscountRateList = roomRateCore.GetDataByRateType(GlobalVariable.RoomRateType.Discount);

                        ModelState.AddModelError("", "End Date can't ealier than start date.");
                        return View(model);
                    }

                    //if (roomRatePolicy.IsExistRoomRatePolicy(model))
                    //{
                    //    //basic rate list init
                    //    model.BasicRateList = roomRateCore.GetDataByRateType(GlobalVariable.RoomRateType.Basic);

                    //    //discount rate list init
                    //    model.DiscountRateList = roomRateCore.GetDataByRateType(GlobalVariable.RoomRateType.Discount);

                    //    ModelState.AddModelError("", "Period has already existed.");
                    //    return View(model);
                    //}

                    if (model.OpertationStatus == GlobalVariable.UpdateStatus.Add)
                    {
                        //insert new room rate
                        model.CreateUser = LoginVariable.UserName;
                        model.UpdateUser = LoginVariable.UserName;
                        roomRatePolicy.InsertData(model);
                        _logHelper.WriteLog("add room rate policy. username:" + model.CreateUser);
                        return RedirectToAction("RatePolicyList", "RoomRate");

                    }
                    else if (model.OpertationStatus == GlobalVariable.UpdateStatus.Update)
                    {
                        //update room rate
                        model.UpdateUser = LoginVariable.UserName;
                        roomRatePolicy.UpdateData(model);

                        _logHelper.WriteLog("update room rate policy success. username:" + model.UpdateUser);

                        return RedirectToAction("RatePolicyList", "RoomRate");

                    }
                }
                else
                {
                    //basic rate list init
                    model.BasicRateList = roomRateCore.GetDataByRateType(GlobalVariable.RoomRateType.Basic);

                    //discount rate list init
                    model.DiscountRateList = roomRateCore.GetDataByRateType(GlobalVariable.RoomRateType.Discount);
                }

            }
            catch (Exception ex)
            {
                _logHelper.WriteLog("RatePolicyDetail(httppost) exception. operator=" + model.OpertationStatus, ex.Message, string.Empty);
            }
            return View(model);

        }

        #endregion
    }
}
