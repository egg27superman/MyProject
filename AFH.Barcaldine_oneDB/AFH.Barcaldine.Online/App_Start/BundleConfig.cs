using System.Web;
using System.Web.Optimization;

namespace AFH.Barcaldine.Online
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-1.6.4.js", "~/Scripts/system.js", "~/Scripts/jquery-ui.js", "~/Scripts/online.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/jquery-ui.css", "~/Content/style.css"));

        }
    }
}