using FamousCroatianConfessionBot.Model;
using FamousCroatianConfessionBot.Bot;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FamousCroatianConfessionBot
{
  [BotAuthentication]
	public class MessagesController : ApiController
	{
    private IFccBot _bot;

    public MessagesController()
    {
      _bot = new FccBot();
    }

		/// <summary>
		/// POST: api/Messages
		/// Receive a message from a user and reply to it
		/// </summary>
		[HttpGet, HttpPost]
		public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
		{
      await FccActivity.Handle(_bot, activity);
			
			// Return response
			return Request.CreateResponse(HttpStatusCode.OK);
    }

   

	}
}
