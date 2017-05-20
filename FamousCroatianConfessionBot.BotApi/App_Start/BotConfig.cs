using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace FamousCroatianConfessionBot
{
	public static class BotConfig
	{
		public static string MS_COGNITIVE_API_KEY { get; private set; }

		public static void Configure()
		{
			MS_COGNITIVE_API_KEY = WebConfigurationManager.AppSettings["MsCognitiveApiKey"];
		}
	}
}