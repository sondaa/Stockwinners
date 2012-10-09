using System.Web;
using System.Web.Optimization;

namespace WebSite
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui*"));

            bundles.Add(new ScriptBundle("~/bundles/upshot").Include(
                        "~/Scripts/upshot.js",
                        "~/Scripts/knockout-2.1.0.js",
                        "~/Scripts/upshot.compat.knockout.js",
                        "~/Scripts/upshot.knockout.extensions.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new ScriptBundle("~/bundles/subscription").Include("~/bundles/jquery", "~/Scripts/picks/subscription.js"));

            bundles.Add(new ScriptBundle("~/bundles/market-radar").Include(
                    "~/Scripts/pages/market-radar/viewmodel.js",
                    "~/Scripts/jquery.signalR-0.5.2.js",
                    "~/Scripts/pages/market-radar/ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/shared").Include(
                "~/Scripts/Shared/indexes.js",
                "~/Scripts/Shared/analytics.js",
                "~/Scripts/Shared/compatibility.js"));
        }
    }
}