using System.Web;
using System.Web.Optimization;

namespace BordaAlgorithm
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/jquery").Include(
                         "~/Scripts/jquery-3.3.1.js",
                         "~/Scripts/jquery-validation/dist/jquery.validate.js"));
            bundles.Add(new StyleBundle("~/adminlte/css").Include(
                      "~/Content/Theme/adminlte/bower_components/bootstrap/dist/css/bootstrap.min.css",
                      "~/Content/Theme/adminlte/bower_components/font-awesome/css/font-awesome.min.css",
                      "~/Content/Theme/adminlte/bower_components/Ionicons/css/ionicons.min.css",
                      "~/Content/Theme/adminlte/bower_components/datatables.net-bs/css/dataTables.bootstrap.min.css",
                      //"~/Content/Theme/adminlte/bower_components/select2/dist/css/select2.css",
                      //"~/Content/Theme/adminlte/bower_components/select2/dist/css/select2_bootstrap.css",
                      "~/Scripts/select2-3.5.4/css/select2.css",
                      "~/Scripts/select2-3.5.4/css/select2_bootstrap.css",
                      "~/Content/Theme/adminlte/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.css",
                      "~/Content/Theme/adminlte/plugins/daterangepicker/daterangepicker.css",
                      "~/Content/Theme/adminlte/plugins/timepicker/bootstrap-timepicker.css",
                      "~/Content/Theme/adminlte/dist/css/AdminLTE.min.css",
                      "~/Content/Theme/adminlte/dist/css/skins/_all-skins.min.css",
                      "~/Content/Theme/font.css"
            ));
            bundles.Add(new Bundle("~/adminlte/js").Include(
             "~/Content/Theme/adminlte/bower_components/jquery/dist/jquery.min.js",
             "~/Scripts/admin-lte/js/app.js",
             "~/Scripts/admin-lte/plugins/fastclick/fastclick.js",
             "~/Scripts/admin-lte/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
             "~/Content/Theme/adminlte/bower_components/bootstrap/dist/js/bootstrap.min.js",
             "~/Content/Theme/adminlte/plugins/timepicker/bootstrap-timepicker.js",
             "~/Content/Theme/adminlte/dist/js/app.min.js",
             "~/Content/Theme/adminlte/bower_components/datatables.net/js/jquery.dataTables.min.js",
             "~/Content/Theme/adminlte/bower_components/datatables.net-bs/js/dataTables.bootstrap.min.js",
             //"~/Content/Theme/adminlte/bower_components/select2/dist/js/select2.js",
             "~/Scripts/select2-3.5.4/js/select2.js",
             "~/Scripts/admin-lte/plugins/daterangepicker/moment.min.js",
             "~/Content/Theme/adminlte/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.js",
              "~/Scripts/admin-lte/plugins/daterangepicker/daterangepicker.js",
             "~/Content/Theme/adminlte/bower_components/jquery-slimscroll/jquery.slimscroll.min.js",
             "~/Scripts/jquery.number.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/dataTable").Include(
                      "~/Scripts/datatable/dataTables.scroller.min.js",
                      "~/Scripts/datatable/export/dataTables.buttons.min.js",
                      "~/Scripts/datatable/export/jszip.min.js",
                      "~/Scripts/datatable/export/pdfmake.min.js",
                      "~/Scripts/datatable/export/vfs_fonts.js",
                      "~/Scripts/datatable/export/buttons.html5.min.js"
                      ));

            bundles.Add(new StyleBundle("~/css/dataTable").Include(
                        "~/Scripts/datatable/export/buttons.dataTables.min.css"
                        //"~/Scripts/datatable/scroller.dataTables.min.css"
                        ));

            bundles.Add(new Bundle("~/bundles/bootbox").Include(
                        "~/Scripts/bootbox.min.js"));

            bundles.Add(new StyleBundle("~/css/my").Include(
                        "~/Content/my.css"));
            BundleTable.EnableOptimizations = false;
        }
    }
}
