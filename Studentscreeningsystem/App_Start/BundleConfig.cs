using System.Web;
using System.Web.Optimization;

namespace Studentscreeningsystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/DataTables/js/jquery.dataTables.min.js",
                          "~/Scripts/jquery-ui-1.10.3.custom.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Scripts/DataTables/css/jquery.dataTables.min.css",
                      "~/Content/site.css",
                      "~/Content/font-awesome.min.css"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/mvcgrid").Include(
    "~/Scripts/jquery-3.0.0.min.js",
    "~/Scripts/gridmvc.min.js",
"~/Scripts/gridmvc.lang.ar.js"
   ));
        }
    }
}