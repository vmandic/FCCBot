using FamousCroatianConfessionBot.Model;
using FamousCroatianConfessionBot.Service;
using System.Web.Http;

namespace FamousCroatianConfessionBot {
  public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
			BotConfig.Configure();
			DataModel.LoadFromFile();

      SlackEndPoint.StartSlackClient();
		}

		protected void Application_End() {
			DataModel.SaveToFile();
		}
	}
}
