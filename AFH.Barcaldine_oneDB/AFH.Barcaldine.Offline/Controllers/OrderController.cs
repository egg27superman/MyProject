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
    public class OrderController : Controller
    {
        //
        // GET: /Order/


        #region OrderList

        public ActionResult OrderList()
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            OrderListModel model = new OrderListModel();

            try
            {
                //Default data: Select all data
                model.OrderListSearch = new OrderListSearchModel();
                model.OrderListSearch.OrderType = 9;
                model.OrderListSearch.ProcessType = 9;
                model.OrderListSearch.OrderStatus = 9;
                
                model.OrderListSearch.OrderDateStart = DateTime.Today;
                model.OrderListSearch.OrderDateEnd = DateTime.Today;


                OrderCore orderCore = new OrderCore();
                model = orderCore.GetSearchResult(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Order List (httpget) exception. ex:" + ex.Message);
                _logHelper.WriteLog("Order List (httpget) exception.", ex.Message, string.Empty);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult OrderList(OrderListModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            try
            {
                OrderCore orderCore = new OrderCore();
                model = orderCore.GetSearchResult(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Order List (httppost) exception. ex:" + ex.Message);
                _logHelper.WriteLog("Order List (httppost) exception.", ex.Message, string.Empty);
            }
            return View(model);
        }
        #endregion


        #region FaultOrderList
        
        public ActionResult FaultOrderList()
        {

            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            FaultOrderListModel model = new FaultOrderListModel();

            try
            {
                //Default data: Select all data
                model.FaultOrderListSearch = new FaultOrderListSearchModel();
                model.FaultOrderListSearch.OrderType = 9;
                model.FaultOrderListSearch.ProcessType = 9;
                model.FaultOrderListSearch.OrderStatus = 9;
                model.FaultOrderListSearch.IsDelete = 0;

                model.FaultOrderListSearch.OrderDateStart = DateTime.Today;
                model.FaultOrderListSearch.OrderDateEnd = DateTime.Today;


                OrderCore orderCore = new OrderCore();
                model = orderCore.GetFaultSearchResult(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Fault Order List (httpget) exception. ex:" + ex.Message);
                _logHelper.WriteLog("Fault Order List (httpget) exception.", ex.Message, string.Empty);
            }
            return View(model);

        }

        [HttpPost]
        public ActionResult FaultOrderList(FaultOrderListModel model)
        {

            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            try
            {
                OrderCore orderCore = new OrderCore();
                
                model = orderCore.GetFaultSearchResult(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Fault Order List (HttpPost) exception. ex:" + ex.Message);
                _logHelper.WriteLog("Fault Order List (HttpPost) exception.", ex.Message, string.Empty);
            }
            return View(model);

        }

        [HttpPost]
        public ActionResult DeleteFaultOrder(string orderID)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());
            try
            {
                FaultOrdersCore fault = new FaultOrdersCore();
                fault.DeleteFault(orderID, LoginVariable.UserName);

                return RedirectToAction("FaultOrderList", "Order");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Delete Fault Order (HttpPost) exception. ex:" + ex.Message);
                _logHelper.WriteLog("Delete Fault Order (HttpPost) exception.", ex.Message, string.Empty);
            }

            return RedirectToAction("OrderDetail", "Order", new { orderID = orderID, sourcefrom = "FaultOrderList" });

        }

        #endregion

        #region OrderDetail

        public ActionResult OrderDetail(string orderID, string sourcefrom)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            OrderDetailModel model = new OrderDetailModel();
            try
            {
                OrderCore orderCore = new OrderCore();
                model = orderCore.GetOrderDetail(orderID);
                model.SourceFrom = sourcefrom;

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Order Detail (httppost) exception. ex:" + ex.Message);
                _logHelper.WriteLog("Order Detail (httppost) exception.", ex.Message, string.Empty);
            }
            return View(model);
        }
       
        #endregion

    }

}
