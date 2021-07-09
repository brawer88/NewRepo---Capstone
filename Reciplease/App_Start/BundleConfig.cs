using System.Web;
using System.Web.Optimization;

namespace Reciplease {
	public class BundleConfig {
		// For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles( BundleCollection bundles ) {
			bundles.Add( new ScriptBundle( "~/bundles/jquery" ).Include(
						"~/Scripts/jquery-{version}.js" ) );

			bundles.Add( new ScriptBundle( "~/bundles/jqueryval" ).Include(
						"~/Scripts/jquery.validate*" ) );

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
			bundles.Add( new ScriptBundle( "~/bundles/modernizr" ).Include(
						"~/Scripts/modernizr-*" ) );

			bundles.Add( new ScriptBundle( "~/bundles/bootstrap" ).Include(
					  "~/Scripts/bootstrap*" ) );
			bundles.Add( new ScriptBundle( "~/bundles/jquery" ).Include(
					  "~/Scripts/jquery-3.3.1.min.js",
					  "~/Scripts/jquery-3.3.1.min.map",
					  "~/Scripts/jquery-3.3.1.min.js",
					  "~/Scripts/jquery-3.3.1.slim.js",
					  "~/Scripts/jquery-3.3.1.slim.min.js",
					  "~/Scripts/jquery-3.3.1.slim.min.map",
					  "~/Scripts/jquery-3.3.1.js" ) );
			bundles.Add( new ScriptBundle( "~/bundles/js" ).Include(
					  "~/Scripts/main.js",
					  "~/Scripts/popper.js") );

			bundles.Add( new StyleBundle( "~/Content/css" ).Include(
					  "~/Content/style2.css",
					  "~/Content/style.css",
					  "~/Content/bootstrap.min.css",
					  "~/Content/site.css",
					  "~/Content/profile.css",
					  "~/Content/cartstyle.css",
					  "~/Content/util1.css" ) );
		}
	}
}
