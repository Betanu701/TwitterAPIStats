using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TwitterAPIStats
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			bool skipBuild = System.Configuration.ConfigurationManager.AppSettings["SkipBuildingSQL"] == "1" ? true : false;
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			RunTwitter(skipBuild);
		}
		
		private async void RunTwitter(bool skipBuild)
			{
				try
				{
					await TwitterConfig.StartTwitterAPI(skipBuild);
				}
				catch (Exception ex)
				{
				// Something failed in thep program. Connection times out after so long. 
				// This will ensure the program keeps running. 
					RunTwitter(true);
				}
		}
	}
}
