using System.Web;
using System.Web.Optimization;

namespace AFH.Barcaldine.Offline
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-1.9.1.js", "~/Scripts/jquery-ui.js", "~/Scripts/uploadfile.js", "~/Scripts/offline.js",
            //     "~/Scripts/language.js", "~/Scripts/jquery.easytree.js", "~/Scripts/plupload.full.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-1.9.1.js", "~/Scripts/jquery-ui.js", "~/Scripts/offline.js",
                 "~/Scripts/language.js", "~/Scripts/jquery.easytree.js"));

            //bundles.Add(new ScriptBundle("~/bundles/productdetailplup").Include("~/Scripts/plupload.js", "~/Scripts/productdetailplup.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css", "~/Content/jquery-ui.css",
                "~/Content/flexnav.css", "~/Content/PagedList.css", "~/Content/ui.easytree.css"));
        }
    }
}