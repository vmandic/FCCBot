using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Threading.Tasks;
using Response = FamousCroatianConfessionBot.Bot.FccBotStrings;

namespace FamousCroatianConfessionBot.Bot
{
  public static class FccActivity
  {
    public static async Task Handle(IFccBot bot, Activity activity)
    {
      try
      {
        if (bot == null) throw new ArgumentNullException(nameof(bot));
        if (activity == null) throw new ArgumentNullException(nameof(activity));

        if (activity.Text != null && activity.Text.Split(' ').Any(x => x.ToLowerInvariant() == FccBotCommands.GOOD_BYE || x.ToLowerInvariant() == FccBotCommands.BYE))
          activity.Type = ActivityTypes.DeleteUserData;

        // Global values
        if (activity.Type == ActivityTypes.Message)
        {
          bot.SetState(new FccBotState(activity));
          var data = bot.State.UserData;
          string text = null;

          // Handle invalid attachments
          if (activity.Attachments.Count > 1 
          || (activity.Attachments.Count == 1 && !activity.Attachments.First().ContentType.Contains("image")))
            throw new InvalidOperationException(Response.I_CAN_HANDLE_ONLY_ONE_ATT);

          // Process a single image attachment
          if (activity.Attachments.Count == 1 && activity.Attachments.First().ContentType.Contains("image"))
            text = await bot.RespondOnImage(activity.Attachments[0]);

          // Create text for a reply message   
          if (text == null)
            text = bot.RespondOnAnyTextWithText(data, activity);

          // Save BotUserData
          bot.State.SetUserData(data);

          // Create a reply message
          await bot.CreateTextReply(activity, text);
        }
        else
          await bot.CreateHandleSystemMessageReply(activity);
      }
      catch (Exception ex)
      {
        // Create an error reply message
        await bot.CreateTextReply(activity, String.Format(Response.WHOOPS_STH_WENT_WRONG_HERES_THE_ERROR, ex.Message));
      }
    }
  }
}