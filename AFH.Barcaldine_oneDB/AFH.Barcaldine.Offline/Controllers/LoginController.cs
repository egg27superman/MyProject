using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AFH.Barcaldine.Models;
using AFH.Barcaldine.Core;
using AFH.Barcaldine.Log;
using AFH.Barcaldine.Common;

namespace AFH.Barcaldine.Offline.Controllers
{
    public class LoginController : Controller
    {
        private DBLogHelper _logHelper;
        public LoginController()
        {
            _logHelper = new DBLogHelper("Offline", "Login");//快速查找哪个模块出问题，Constant.Log.Login可用action name代替
        }
        //
        // GET: /Login/

        public ActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginPage(LoginModule loginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //bool result = false;
                    //int usertype;
                    CheckLoginInfoCore myCheckLoginCore = new CheckLoginInfoCore();
                    int userID = myCheckLoginCore.CheckLogin(loginModel);
                    //usertype = myCheckLoginCore.UserType(loginModel);


                    if (userID > 0)
                    {
                        LoginVariable.UserName = loginModel.UserName;
                        LoginVariable.UserID = userID;
                        HttpCookie _cookie = new HttpCookie("AIUG.UserName", loginModel.UserName);
                        _cookie.Expires = DateTime.Now.AddHours(1);
                        Response.SetCookie(_cookie);

                        //添加成功转向主页  
                        _logHelper.WriteLog("Login sucessed");

                        //ViewBag.Menu = "aaa";



                        return RedirectToAction("Welcome", "Welcome");
                    }
                    //else if (result & usertype == 1)
                    //{
                    //    LoginVariable.UserName = loginModel.UserName;
                    //    HttpCookie _cookie = new HttpCookie("AIUG.UserName", loginModel.UserName);
                    //    _cookie.Expires = DateTime.Now.AddHours(1);
                    //    Response.SetCookie(_cookie);
                    //    ViewBag.Menu = "bb";

                    //    //添加成功转向主页  
                    //    _logHelper.WriteLog("Login sucessed");
                    //    return RedirectToAction("RoomRateList", "RoomRate");
                       
                    //}
                    else
                    {
                        //返回注册页面  
                        ModelState.AddModelError("Password", "Username or Password is invalid");
                        _logHelper.WriteLog("Login failed: Username or Password is invalid");
                    }


                }

                return View(loginModel);


            }
            catch (Exception ex)
            {
                _logHelper.WriteLog("Login exception", ex.Message, string.Empty);
                return View();
            }




        }



    }
}
