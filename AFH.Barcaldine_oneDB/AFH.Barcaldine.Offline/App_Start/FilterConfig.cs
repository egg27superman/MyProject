using System.Web;
using System.Web.Mvc;

namespace AFH.Barcaldine.Offline
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}