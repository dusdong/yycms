using System.Web;
using System.Web.Optimization;

namespace yycms.admin
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/Scripts/area.js",
                        "~/Scripts/crypto.js",
                        "~/Scripts/angular.min.js",
                        "~/Scripts/angular-resource.min.js",
                        "~/Scripts/angular-route.min.js",
                        "~/Scripts/angular-sanitize.min.js",
                        "~/Scripts/angular-messages.min.js",
                        "~/Scripts/angular-cookies.min.js",
                        "~/Scripts/jquery-2.1.4.min.js",
                        "~/Scripts/screenfull.min.js",
                        "~/Scripts/toastr.min.js",
                        "~/Scripts/jquery.jqprint-0.3.js",
                        "~/Scripts/excellentexport.min.js",
                        "~/Scripts/nprogress.js",
                        "~/Scripts/jquery.signalR-2.2.0.min.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/jquery.cookie.js",
                        "~/Scripts/app.js",
                        "~/Scripts/api.js",
                        "~/Scripts/intro.min.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/introjs.min.css",
                      "~/Content/bootstrap.min.css",
                      "~/Content/animate.css",
                      "~/Content/eleganticons.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/hover-min.css",
                      "~/Content/toastr.min.css",
                      "~/Content/Site.css"));

            // 将 EnableOptimizations 设为 false 以进行调试。有关详细信息，
            // 请访问 http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
