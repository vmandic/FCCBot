using Microsoft.Bot.Connector;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Response = FamousCroatianConfessionBot.Bot.FccBotStrings;

namespace FamousCroatianConfessionBot.Bot
{
  public interface IFccBot
  {
    string RespondOnAnyTextWithText(FccBotUserData data, Activity activity);
    Task<string> RespondOnImage(Attachment att);
    void SetState(FccBotState state);
    Task CreateTextReply(Activity activity, string text);
    Task CreateHandleSystemMessageReply(Activity activity);
    FccBotState State { get; }
  }

  public class FccBot : IFccBot
  {
    private FccBotState _state;

    public FccBotState State => _state;

    public void SetState(FccBotState state) => _state = state;

    public async Task<string> RespondOnAttachmentWithText(Attachment att)
    {
      if (att.ContentType.Contains("image"))
      {
        return await RespondOnImage(att);
      }
      else
      {
        return Response.UNSUPPORTED_ATTACHMENT_TYPE;
      }
    }

    public async Task<string> RespondOnImage(Attachment att)
    {
      try
      {
        var imgStream = (await WebRequest.Create(att.ContentUrl).GetResponseAsync()).GetResponseStream();
        // TODO: limit to maybe 3 MB
        return await RecognizeEmotionsFromPortraitImage(imgStream);
      }
      catch (Exception ex)
      {
        return String.Format(Response.WHOOPS_STH_WENT_WRONG_HERES_THE_ERROR, ex.Message);
      }
    }

    public string RespondOnAnyTextWithText(FccBotUserData data, Activity activity)
    {
      try
      {
        var strReplyMessage = new StringBuilder();
        if (data.AskedForUserName == false)
        {
          strReplyMessage.Append($"Hey what's up! :)");
          strReplyMessage.Append($"\n\r");
          strReplyMessage.Append($"I'd like to know who are you?");

          // Set BotUserData
          data.AskedForUserName = true;
        }
        else
        {
          if (data.UserName == null) // Name was never provided
          {
            // If we have asked for a username but it has not been set
            // the current response is the user name
            strReplyMessage.Append($"Hey there {activity.Text}! Speak up... :)");

            // Set BotUserData
            data.UserName = activity.Text;
          }
          else // Name was provided
          {
            strReplyMessage.Append($"{data.UserName}, you said and I'll repeat: \"{activity.Text}\"\n\r");
            strReplyMessage.Append($"If you'd like to see something smart... send me a selfie photo! :)");
          }
        }
        return strReplyMessage.ToString();
      }
      catch (Exception ex)
      {
        return String.Format(Response.WHOOPS_STH_WENT_WRONG_HERES_THE_ERROR, ex.Message);
      }
    }

    private static string PrintEmotion(Emotion e)
    {
      var s = new[]
      {
        $"ˇˇˇˇˇˇˇ SCORE ˇˇˇˇˇˇˇ",
        $"Anger:     {PrintFloat(e.Scores.Anger)}",
        $"Contempt:  {PrintFloat(e.Scores.Contempt)}",
        $"Disgust:   {PrintFloat(e.Scores.Disgust)}",
        $"Fear:      {PrintFloat(e.Scores.Fear)}",
        $"Happiness: {PrintFloat(e.Scores.Happiness)}",
        $"Neutral:   {PrintFloat(e.Scores.Neutral)}",
        $"Sadness:   {PrintFloat(e.Scores.Sadness)}",
        $"Surprise:  {PrintFloat(e.Scores.Surprise)}"
      };

      return System.String.Join("\n\r", s);
    }

    public static async Task<string> RecognizeEmotionsFromPortraitImage(System.IO.Stream imgStream)
    {
      var emotionServiceClient = new EmotionServiceClient(BotConfig.MS_COGNITIVE_API_KEY);
      var emotions = await emotionServiceClient.RecognizeAsync(imgStream);
      var strEmotions = emotions.Select(PrintEmotion);

      return string.Join("\n\r\n\r", strEmotions);
    }

    public static async Task<string> RecognizeEmotionsFromPortraitImage(string imageUrl)
    {
      var emotionServiceClient = new EmotionServiceClient(BotConfig.MS_COGNITIVE_API_KEY);
      var emotions = await emotionServiceClient.RecognizeAsync(imageUrl);
      var strEmotions = emotions.Select(PrintEmotion);

      return string.Join("\n\r\n\r", strEmotions);
    }

    private static string PrintFloat(float f) => f.ToString("0.########");

    public async Task CreateTextReply(Activity activity, string text)
    {
      Activity replyMessage = activity.CreateReply(text);

      if (replyMessage != null)
        await new ConnectorClient(new Uri(activity.ServiceUrl)).Conversations.ReplyToActivityAsync(replyMessage);
    }

    public async Task CreateHandleSystemMessageReply(Activity activity)
    {
      Activity replyMessage = HandleSystemMessage(activity);

      if (replyMessage != null)
      {
        ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
        await connector.Conversations.ReplyToActivityAsync(replyMessage);
      }
    }

    private static Activity HandleSystemMessage(Activity message)
    {
      if (message.Type == ActivityTypes.DeleteUserData)
      {
        // Get BotUserData
        StateClient sc = message.GetStateClient();
        BotData userData = sc.BotState.GetPrivateConversationData
        (
          message.ChannelId,
          message.Conversation.Id,
          message.From.Id
        );

        // Set BotUserData
        userData.SetProperty(nameof(FccBotUserData), new FccBotUserData());

        // Save BotUserData
        sc.BotState.SetPrivateConversationData
        (
          message.ChannelId,
          message.Conversation.Id,
          message.From.Id,
          userData
        );

        // Create a reply message
        ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
        Activity replyMessage = message.CreateReply("Personal data has been deleted.");

        return replyMessage;
      }
      else if (message.Type == ActivityTypes.ConversationUpdate)
      {
        // Handle conversation state changes, like members being added and removed
        // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
        // Not available in all channels
      }
      else if (message.Type == ActivityTypes.ContactRelationUpdate)
      {
        // Handle add/remove from contact lists
        // Activity.From + Activity.Action represent what happened
      }
      else if (message.Type == ActivityTypes.Typing)
      {
        // Handle knowing tha the user is typing
      }
      else if (message.Type == ActivityTypes.Ping)
      {
      }

      return null;
    }
  }
}
