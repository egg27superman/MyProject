using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;
using System.Globalization;
using AFH.Barcaldine.Common;


namespace AFH.Barcaldine.Offline.Controllers
{
    public class LocalizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //未登录时重定向到登录页面
            var cookie = filterContext.HttpContext.Request.Cookies["AIUG.UserName"];
            if (cookie != null)
            {

                LoginVariable.UserName = cookie.Value;
                if (string.IsNullOrEmpty(LoginVariable.UserName))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Login",
                        action = "Login"
                    }));
                    return;
                }

            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Login",
                    action = "Login"
                }));
                return;
            }



        }

    }
}
