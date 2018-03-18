using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;//需要加的
using System.Configuration;//需要加的

using AFH.Barcaldine.Models;//需要加的
using AFH.Barcaldine.Core;//需要加的
using AFH.Barcaldine.Log;//需要加的
using AFH.Barcaldine.Common;//需要加的

namespace AFH.Barcaldine.Offline.Controllers
{
       [LocalizationAttribute]
    public class UserAdminController : Controller
    {

        private DBLogHelper _logHelper;
        public UserAdminController()
        {
            _logHelper = new DBLogHelper("Offline", "Login");// _logHelper = new DBLogHelper(Constant.Log.Offline, Constant.Log.User);
        }

        public ActionResult RoomRateList()
        {
            /*
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
             */
            return View();
        }


        #region UserList and UserAdmin Management
        //
        // GET: /User/

        public ActionResult UserList()
        {
            UserListModule model = new UserListModule();
            model.UserSearch = new UserSearchModule();
            model.UserSearch.UserID = null;
            model.UserResultList = new List<UserResultListModule>();

            return View(model);
        }

        [HttpPost]
        public ActionResult UserList(UserListModule model)
        {
            try
            {
                UserCore user = new UserCore();
                model.UserResultList = user.GetUserList(model.UserSearch);
            }
            catch (Exception ex)
            {
                _logHelper.WriteLog("UserList exception.", ex.Message, string.Empty);
            }
            return View(model);
        }

        #endregion

        #region Privilege

        public ActionResult UserPrivilege(string opertation, string id)
        {
            UserUpdateModule model = new UserUpdateModule();
            UserCore user = new UserCore();
            try
            {
                if (opertation == GlobalVariable.UpdateStatus.Add)
                {
                    model.OpertationStatus = GlobalVariable.UpdateStatus.Add;
                    model.IsDelete = false;
                    model.UserNameSearch = new UserResultSearchModule();
                    model.UserNameSearch.GetUserNameList = user.GetUsernameList(string.Empty);
                    string userid = Convert.ToString(model.UserNameSearch.UserID);
                    model.UserPrivilegeList = user.SetUserpribilegeByID(userid);
                }
                else if (opertation == GlobalVariable.UpdateStatus.Update)
                {
                    
                    model = user.GetUserByID(id);
                    model.OpertationStatus = GlobalVariable.UpdateStatus.Update;
                    model.UserPrivilegeList = user.GetUserpribilegeByID(id);
                }
            }
            catch (Exception ex)
            {
                _logHelper.WriteLog("UserDetail exception.", ex.Message, string.Empty);
            }

            return View(model);

        }

        [HttpPost]
        public ActionResult UserPrivilege(UserUpdateModule model)
        {
            try
            {
                UserCore userCore = new UserCore();
                if (model.UserNameSearch == null)
                {
                    model.UserNameSearch = new UserResultSearchModule();
                }
                model.UserNameSearch.GetUserNameList = userCore.GetUsernameList(string.Empty);//数据可能丢失，需再取一次

                if (ModelState.IsValid)
                {
                   

                    if (model.OpertationStatus == GlobalVariable.UpdateStatus.Add)
                    {
                        //insert new user

                        //save
                        model.UpdateUser = LoginVariable.UserName;
                        userCore.AddUserPrivilege(model);
                        _logHelper.WriteLog("add user success. username:" + model.UserName);
                        return RedirectToAction("UserList", "UserAdmin");

                    }
                    else if (model.OpertationStatus == GlobalVariable.UpdateStatus.Update)
                    {
                        //update user
                        model.UpdateUser = LoginVariable.UserName;
                        userCore.UpdateUser(model);

                        _logHelper.WriteLog("update user success. username:" + model.UserName);

                        return RedirectToAction("UserList", "UserAdmin");

                    }
                }

            }
            catch (Exception ex)
            {
                _logHelper.WriteLog("UserDetail(httppost) exception. operator=" + model.OpertationStatus, ex.Message, string.Empty);
            }


            return View(model);
        }

        #endregion

        #region Reset Password
        public ActionResult ResetPassword(string opertation, string id)
        {
            UserUpdateModule model = new UserUpdateModule();
            try
            {

                UserCore user = new UserCore();
                model = user.GetUserByID(id);
                model.OpertationStatus = GlobalVariable.UpdateStatus.Update;
                model.UpdateUser = LoginVariable.UserName;
                model.Password = Common.SecurityHelper.Encode(ConfigurationManager.AppSettings["DefaultPassword"].ToString());  //加密
                user.UpdateUser(model);
                _logHelper.WriteLog("Reset user Password success. username:" + model.UserName);

                ViewBag.SucessMessage = "The password has been reset";
            }
            catch (Exception ex)
            {
                _logHelper.WriteLog("ResetPassword exception.", ex.Message, string.Empty);
            }
            return View("UserDetail", model);
        }
        #endregion

        #region UserChangePassword

        public ActionResult UserChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserChangePassword(ChangePasswordModule model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //current password
                    UserCore core = new UserCore();

                    //Check data
                    string currentPassword = core.GetUserPasswordByName(LoginVariable.UserName);
                    if (model.CurrentPassword != Common.SecurityHelper.Decode(currentPassword))
                    {
                        ModelState.AddModelError("", "The current password is wrong.");
                        return View(model);
                    }

                    if (model.NewPassword != model.ConfirmPassword)
                    {
                        ModelState.AddModelError("", "The new password and confirmation password do not match.");
                        return View(model);
                    }

                    //save new password
                    model.NewPassword = Common.SecurityHelper.Encode(model.NewPassword);
                    model.UpdateUser = LoginVariable.UserName;
                    model.UserName = LoginVariable.UserName;
                    core.UpdatePassword(model);

                    _logHelper.WriteLog("change password success. username:" + model.UserName);

                    ViewBag.SucessMessage = "Your password has been changed";
                }

                return View();
            }
            catch (Exception ex)
            {
                _logHelper.WriteLog("change password exception.", ex.Message, string.Empty);
            }

            return View();
        }
        #endregion

    }
}
