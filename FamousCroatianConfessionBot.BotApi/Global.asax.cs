using FamousCroatianConfessionBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace FamousCroatianConfessionBot
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
			DataModel.LoadFromFile();
		}

		protected void Application_End() {
			DataModel.SaveToFile();
		}
	}
}
