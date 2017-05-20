using FamousCroatianConfessionBot.Bot;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Response = FamousCroatianConfessionBot.Bot.FccBotStrings;

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
      await HandleChatActivity(activity);
			
			// Return response
			return Request.CreateResponse(HttpStatusCode.OK);
    }

    async Task HandleChatActivity(Activity activity)
    {
      try
      {
        // Global values
        if (activity.Type == ActivityTypes.Message)
        {
          _bot.SetState(new FccBotState(activity));
          var data = _bot.State.UserData;
          string text = null;

          if (activity.Attachments.Any())
          {
            // Process attachments
            text = await _bot.RespondOnImage(activity.Attachments[0]);
          }

          if (text == null)
          {
            // Create text for a reply message   
            text = _bot.RespondOnAnyTextWithText(data, activity);
          }

          // Save BotUserData
          _bot.State.SetUserData(data);

          // Create a reply message
          await _bot.CreateTextReply(activity, text);
        }
        else
          await _bot.CreateHandleSystemMessageReply(activity);
      }
      catch (Exception ex)
      {
        // Create an error reply message
        await _bot.CreateTextReply(activity, String.Format(Response.WHOOPS_STH_WENT_WRONG_HERES_THE_ERROR, ex.Message));
      }
    }

	}
}
