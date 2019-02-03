using System.Web;
using System.Web.Optimization;

namespace Rela_project
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                   "~/Content/bootstrap.css",
                   "~/Content/font-awesome.css",
                    "~/Content/toastr.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/Login").Include(
                "~/Scripts/jquery-3.3.1.js",
                "~/Scripts/umd/popper.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/toastr.js",
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/jquery.unobtrusive-ajax.js"
                ));

            bundles.Add(new StyleBundle("~/Content/layout").Include(
                   "~/Content/bootstrap.css",
                   "~/Content/font-awesome.css",
                    "~/Content/toastr.css"
                ));

            bundles.Add(new StyleBundle("~/Content/login").Include(
                   "~/Content/bootstrap.css",
                   "~/Content/font-awesome.css",
                    "~/Content/toastr.css",
                    "~/Content/Site.css"
                ));


            bundles.Add(new ScriptBundle("~/bundles/layout").Include(
                "~/Scripts/jquery-3.3.1.js",
                "~/Scripts/umd/popper.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/toastr.js",
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/jquery.unobtrusive-ajax.js"
            ));
            BundleTable.EnableOptimizations = true;
        }
    }
}
