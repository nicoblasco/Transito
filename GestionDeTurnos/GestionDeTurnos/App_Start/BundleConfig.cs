using System.Web;
using System.Web.Optimization;

namespace GestionDeTurnos
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery.validate.unobtrusive.js",
                        "~/Scripts/expressive.annotations*",
                        "~/Scripts/CustomValidation.js"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/sol.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/moment-with-locales.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/bootstrap-datetimepicker.js",
                      "~/Scripts/metisMenu.js",
                      "~/Scripts/jquery.dataTables.js",
                      "~/Scripts/dataTables.bootstrap.min.js",
                      "~/Scripts/sb-admin-2.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/multipartInAjaxBegin.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/Scripts/ini.js",
                      "~/Scripts/jquery.form.js",
                      "~/Scripts/lobibox.js",
                      "~/Scripts/messageboxes.js",
                      "~/Scripts/notifications.js",
                      "~/Scripts/fnReloadAjax.js",
                      "~/Scripts/jquery.autocomplete.js",
                      //"~/Scripts/jquery.datetimepicker.full.min.js",                      
                      "~/Scripts/select2.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      //"~/Content/bootstrap-theme.min.css",
                      "~/Content/style.css",
                      "~/Content/metisMenu.min.css",
                      "~/Content/sol.css",
                      "~/Content/dataTables.bootstrap.css",
                      "~/Content/sb-admin-2.css",
                      "~/Content/dataTables.responsive.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/lobibox.css",
                      "~/Content/bootstrap-datetimepicker.css",
                      "~/Content/jquery.datetimepicker.min.css",
                      "~/Content/select2.css"
                      ));
        }
    }
}
