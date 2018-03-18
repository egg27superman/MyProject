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
    public class WineController : Controller
    {
        //
        // GET: /Wine/

        #region WineCategory

        public ActionResult WineCategoryList()
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());
            
            WineCategoryListModel model = new WineCategoryListModel();
            try
            {
                //Default data: Select all data
                WineCategoryCore wineCategoryCore = new WineCategoryCore();
                model.WineCategorySearch = new WineCategorySearchModel();
                model.WineCategorySearch.IsDelete = "9";
                model.WineCategoryResult = wineCategoryCore.GetSearchData(model.WineCategorySearch);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Wine Category list (httpget) exception. ex:" + ex.Message);
                _logHelper.WriteLog("Wine Category list (httpget) exception.", ex.Message, string.Empty);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult WineCategoryList(WineCategoryListModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            try
            {

                WineCategoryCore wineCategoryCore = new WineCategoryCore();
                model.WineCategoryResult = wineCategoryCore.GetSearchData(model.WineCategorySearch);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Wine Category List (httpget) exception:" + ex.Message);
                _logHelper.WriteLog("Wine Category List (httpget) exception.", ex.Message, string.Empty);
            }

            return View(model);
        }

        public ActionResult WineCategoryDetail(string opertation, string id)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            WineCategoryDetailModel wineCategory = new WineCategoryDetailModel();
            try
            {
                if (opertation == GlobalVariable.UpdateStatus.Add)
                {
                    //add
                    wineCategory.OpertationStatus = opertation;


                }
                else if (opertation == GlobalVariable.UpdateStatus.Update)
                {
                    //update
                    WineCategoryCore winecategoryCore = new WineCategoryCore();
                    wineCategory = winecategoryCore.GetDataByID(id); 

                    wineCategory.OpertationStatus = opertation;
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Wine Category Detail(httpget) exception:" + ex.Message);
                _logHelper.WriteLog("Wine Category Detail(httpget) exception. ", ex.Message, string.Empty);
            }
            return View(wineCategory);
        }

        [HttpPost]
        public ActionResult WineCategoryDetail(WineCategoryDetailModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());
            try
            {
                if (ModelState.IsValid)
                {
                    WineCategoryCore wineCategory = new WineCategoryCore();
                    if (model.OpertationStatus == GlobalVariable.UpdateStatus.Add)
                    {
                        //insert new wine category
                        model.CreateUser = LoginVariable.UserName;
                        model.UpdateUser = LoginVariable.UserName;
                        wineCategory.InsertData(model);
                        _logHelper.WriteLog("add wine category. username:" + model.CreateUser);
                        return RedirectToAction("WineCategoryList", "Wine");

                    }
                    else if (model.OpertationStatus == GlobalVariable.UpdateStatus.Update)
                    {
                        //update wine category
                        model.UpdateUser = LoginVariable.UserName;
                        wineCategory.UpdateData(model);

                        _logHelper.WriteLog("update wine category success. username:" + model.UpdateUser);

                        return RedirectToAction("WineCategoryList", "Wine");

                    }
                }

            }
            catch (Exception ex)
            {
                _logHelper.WriteLog("WineCategoryDetail(httppost) exception. operator=" + model.OpertationStatus, ex.Message, string.Empty);
            }
            return View(model);

        }

        #endregion

        #region Product


        public ActionResult ProductList()
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            ProductListModel model = new ProductListModel();
            try
            {
                //wine category droplist init
                WineCategoryCore wineCategoryCore = new WineCategoryCore();
                model.ProductSearch = new ProductSearchModel();
                model.ProductSearch.GetWineCategoryList = wineCategoryCore.GetCategoryList(string.Empty, true);

                //default data: select all data
                model.ProductSearch.CategoryID = -1;
                model.ProductSearch.IsDelete = "9";
                ProductCore product = new ProductCore();
                model.ProductResult = product.GetSearch(model.ProductSearch);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "ProductList (httpget) exception. ex:" + ex.Message);
                _logHelper.WriteLog("ProductList (httpget) exception.", ex.Message, string.Empty);
            }
            return View(model);

        }

        [HttpPost]
        public ActionResult ProductList(ProductListModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            try
            {
                WineCategoryCore wineCategoryCore = new WineCategoryCore();
                model.ProductSearch.GetWineCategoryList = wineCategoryCore.GetCategoryList(model.ProductSearch.CategoryID.ToString(), true);

                ProductCore product = new ProductCore();
                model.ProductResult = product.GetSearch(model.ProductSearch);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Product List (httpget) exception:" + ex.Message);
                _logHelper.WriteLog("Product List (httpget) exception.", ex.Message, string.Empty);
            }

            return View(model);
        }


        public ActionResult ProductDetail(string opertation, string id)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());
            
            ProductDetailModel product = new ProductDetailModel();

            WineCategoryCore wineCategoryCore = new WineCategoryCore();
            ProductCore productCore = new ProductCore();


            try
            {
                if (opertation == GlobalVariable.UpdateStatus.Add)
                {
                    //wine category init
                    product.GetWineCategoryList = wineCategoryCore.GetCategoryList(string.Empty, false);

                    //Describle init
                    product.Describle = new List<ProductDescribleModel>();
                    string[] languageKey = ConfigurationManager.AppSettings["LanguageKey"].ToString().Split('|');
                    for (int i = 0; i < languageKey.Length; i++)
                    {
                        ProductDescribleModel desc = new ProductDescribleModel();
                        desc.Language = languageKey[i];
                        desc.DescribleDetail = new DescribleDetailModel();
                        product.Describle.Add(desc);
                    }

                    //Wine image init
                    product.WineImages = new List<WineImageModel>();
                    WineImageModel listImage = new WineImageModel();
                    listImage.ImageType = GlobalVariable.WineImageType.List;
                    product.WineImages.Add(listImage);

                    WineImageModel detailImage = new WineImageModel();
                    detailImage.ImageType = GlobalVariable.WineImageType.Detail;
                    product.WineImages.Add(detailImage);

                    product.OpertationStatus = opertation;

                }
                else if (opertation == GlobalVariable.UpdateStatus.Update)
                {
                    product = productCore.GetWineByID(id);

                    //wine category init
                    product.GetWineCategoryList = wineCategoryCore.GetCategoryList(product.WineCategoryID.ToString(), false);

                    product.OpertationStatus = opertation;
                }


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "ProductDetail(httpget) exception:" + ex.Message);
                _logHelper.WriteLog("ProductDetail(httpget) exception. ", ex.Message, string.Empty);
            }
            return View(product);           
        }

        [HttpPost]
        public ActionResult ProductDetail(ProductDetailModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());
            ProductCore product = new ProductCore();
            WineCategoryCore wineCategoryCore = new WineCategoryCore();
            try
            {
                if (ModelState.IsValid)
                {
                    
                    if (model.OpertationStatus == GlobalVariable.UpdateStatus.Add)
                    {
                        model.CreateUser = LoginVariable.UserName;
                        model.UpdateUser = LoginVariable.UserName;

                        product.InsertData(model);

                        _logHelper.WriteLog("add wine product success. username:" + model.CreateUser);
                        return RedirectToAction("ProductList", "Wine");

                    }
                    else if (model.OpertationStatus == GlobalVariable.UpdateStatus.Update)
                    {
                        //update user
                        model.UpdateUser = LoginVariable.UserName;

                        product.UpdateData(model);

                        _logHelper.WriteLog("update wine product success. username:" + model.UpdateUser);

                        return RedirectToAction("ProductList", "Wine");

                    }
                }
                else
                {
                    model.GetWineCategoryList = wineCategoryCore.GetCategoryList(model.WineCategoryID.ToString(), false);

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "ProductDetail save data exception:" + ex.Message);
                _logHelper.WriteLog("ProductDetail(httppost) exception. operator=" + model.OpertationStatus, ex.Message, string.Empty);
            }
            return View(model);
        }



        #endregion

        #region Shipping


        public ActionResult ShippingList()
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            ShippingListModel model = new ShippingListModel();
            try
            {
                //Default data: Select all data
                ShippingCore shipping = new ShippingCore();
                model.ShippingSearch = new ShippingSearchModel();
                model.ShippingSearch.IsDelete = "9";
                model.ShippingSearch.State = "9";
                model.ShippingResult = shipping.GetSearchData(model.ShippingSearch);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Shipping list (httpget) exception. ex:" + ex.Message);
                _logHelper.WriteLog("Shipping list (httpget) exception.", ex.Message, string.Empty);
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult ShippingList(ShippingListModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            try
            {

                ShippingCore shippingCore = new ShippingCore();
                model.ShippingResult = shippingCore.GetSearchData(model.ShippingSearch);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Shipping List (httpget) exception:" + ex.Message);
                _logHelper.WriteLog("Shipping List (httpget) exception.", ex.Message, string.Empty);
            }

            return View(model);
        }

        public ActionResult ShippingDetail(string opertation, string id)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());

            ShippingDetailModel shipping = new ShippingDetailModel();
            try
            {
                if (opertation == GlobalVariable.UpdateStatus.Add)
                {
                    //add
                    shipping.OpertationStatus = opertation;


                }
                else if (opertation == GlobalVariable.UpdateStatus.Update)
                {
                    //update
                    ShippingCore shippingCore = new ShippingCore();
                    shipping = shippingCore.GetDataByID(id);

                    shipping.OpertationStatus = opertation;
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Shipping Detail(httpget) exception:" + ex.Message);
                _logHelper.WriteLog("Shipping Detail(httpget) exception. ", ex.Message, string.Empty);
            }
            return View(shipping);
            
        }

        [HttpPost]
        public ActionResult ShippingDetail(ShippingDetailModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Offline, this.ControllerContext.RouteData.Values["action"].ToString());
            try
            {
                if (ModelState.IsValid)
                {
                    ShippingCore shipping = new ShippingCore();
                    if (model.OpertationStatus == GlobalVariable.UpdateStatus.Add)
                    {

                        if (shipping.IsExistState(model.State))
                        {
                            ModelState.AddModelError("", "The state have existed");
                            return View(model);
                        }

                        //insert new wine category
                        model.CreateUser = LoginVariable.UserName;
                        model.UpdateUser = LoginVariable.UserName;
                        shipping.InsertData(model);
                        _logHelper.WriteLog("add shipping rate. username:" + model.CreateUser);
                        return RedirectToAction("ShippingList", "Wine");

                    }
                    else if (model.OpertationStatus == GlobalVariable.UpdateStatus.Update)
                    {
                        //update wine category
                        model.UpdateUser = LoginVariable.UserName;
                        shipping.UpdateData(model);

                        _logHelper.WriteLog("update shipping rate. username:" + model.UpdateUser);

                        return RedirectToAction("ShippingList", "Wine");

                    }
                }

            }
            catch (Exception ex)
            {
                _logHelper.WriteLog("ShippingDetail(httppost) exception. operator=" + model.OpertationStatus, ex.Message, string.Empty);
            }
            return View(model);

        }

        #endregion
    }
}
