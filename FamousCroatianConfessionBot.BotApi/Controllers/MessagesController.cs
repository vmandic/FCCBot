using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace FamousCroatianConfessionBot {

  [BotAuthentication]
  public class MessagesController : ApiController {
    
    #region Types

    class UData {
      public string UserName { get; set; }
      public bool AskedForUserName { get; set; }
    }

    class State : IDisposable {
      Activity _activity = null;
      StateClient _sc = null;
      BotData _botData = null;
      public State( Activity activity ) {
        // Get any saved values
        _activity = activity;
        _sc = activity.GetStateClient();
        _botData = _sc.BotState.GetPrivateConversationData
        (
          activity.ChannelId,
          activity.Conversation.Id,
          activity.From.Id
        );
      }

      public UData Get() => _botData.GetProperty<UData>( nameof( UData ) ) ?? new UData();

      public void Set( UData data ) {
        _botData.SetProperty( nameof( UData ), data );
        _sc.BotState.SetPrivateConversationData
        (
          _activity.ChannelId,
          _activity.Conversation.Id,
          _activity.From.Id, _botData
        );
      }

      public void Dispose() {
        _sc.Dispose();
      }
    }

    string MsCognitiveApiKey = System.Web.Configuration.WebConfigurationManager.AppSettings["MsCognitiveApiKey"];

    #endregion

    string RespondOnText( UData data, Activity activity ) {
      StringBuilder strReplyMessage = new StringBuilder();
      if ( data.AskedForUserName == false ) {
        strReplyMessage.Append( $"Hej! Šta ima!" );
        strReplyMessage.Append( $"\n\r" );
        strReplyMessage.Append( $"Šta god da kažeš, koga zanima?" );

        // Set BotUserData
        data.AskedForUserName = true;
      } else {
        if ( data.UserName == null ) // Name was never provided
        {
          // If we have asked for a username but it has not been set
          // the current response is the user name
          strReplyMessage.Append( $"Hello {activity.Text}!" );

          // Set BotUserData
          data.UserName = activity.Text;
        } else // Name was provided
          {
          strReplyMessage.Append( $"{data.UserName}, You said: {activity.Text}" );
        }
      }
      return strReplyMessage.ToString();
    }

    async Task<string> RespondOnImage( Attachment att ) {
      var isImage = att.ContentType.Contains( "image" );
      if ( isImage ) {
        var emotionServiceClient = new EmotionServiceClient( MsCognitiveApiKey );
        try {
          var request = HttpWebRequest.Create( att.ContentUrl );
          var response = request.GetResponse();
          var imgStream = response.GetResponseStream();
          var emotions = await emotionServiceClient.RecognizeAsync( imgStream );
          var strEmotions = emotions.Select( PrintEmotion );
          return string.Join( "\n\r\n\r", strEmotions );
        } catch ( Exception ex ) {
          return ex.Message;
        }
      }
      return null;
    }

    string PrintEmotion( Emotion e ) {
      var s = new[] {
        $" ˇˇˇˇˇˇˇ SCORE ˇˇˇˇˇˇˇ",
        $"Anger:     {PrintFloat(e.Scores.Anger)}",
        $"Contempt:  {PrintFloat(e.Scores.Contempt)}",
        $"Disgust:   {PrintFloat(e.Scores.Disgust)}",
        $"Fear:      {PrintFloat(e.Scores.Fear)}",
        $"Happiness: {PrintFloat(e.Scores.Happiness)}",
        $"Neutral:   {PrintFloat(e.Scores.Neutral)}",
        $"Sadness:   {PrintFloat(e.Scores.Sadness)}",
        $"Surprise:  {PrintFloat(e.Scores.Surprise)}"
      };
      return System.String.Join( "\n\r", s );
    }

    string PrintFloat( float f ) => f.ToString( "0.########" );

    /// <summary>
    /// POST: api/Messages
    /// Receive a message from a user and reply to it
    /// </summary>
    [HttpGet, HttpPost]
    public async Task<HttpResponseMessage> Post( [FromBody]Activity activity ) {
      // Global values
      if ( activity.Type == ActivityTypes.Message ) {
        var state = new State( activity );
        var data = state.Get();
        string text = null;

        if ( activity.Attachments.Any() ) {
          // Process attachments
          text = await RespondOnImage( activity.Attachments[0] );
        }
        
        if ( text == null ) {
          // Create text for a reply message   
          text = RespondOnText( data, activity );
        }

        // Save BotUserData
        state.Set( data );

        // Create a reply message
        ConnectorClient connector = new ConnectorClient( new Uri( activity.ServiceUrl ) );
        Activity replyMessage = activity.CreateReply( text );

        await connector.Conversations.ReplyToActivityAsync( replyMessage );
      } else {
        Activity replyMessage = HandleSystemMessage( activity );
        if ( replyMessage != null ) {
          ConnectorClient connector = new ConnectorClient( new Uri( activity.ServiceUrl ) );
          await connector.Conversations.ReplyToActivityAsync( replyMessage );
        }
      }

      // Return response
      var response = Request.CreateResponse( HttpStatusCode.OK );
      return response;
    }

    Activity HandleSystemMessage( Activity message ) {
      if ( message.Type == ActivityTypes.DeleteUserData ) {
        // Get BotUserData
        StateClient sc = message.GetStateClient();
        BotData userData = sc.BotState.GetPrivateConversationData(
            message.ChannelId, message.Conversation.Id, message.From.Id );
        // Set BotUserData
        userData.SetProperty( nameof( UData ), new UData() );
        // Save BotUserData
        sc.BotState.SetPrivateConversationData(
            message.ChannelId, message.Conversation.Id, message.From.Id, userData );
        // Create a reply message
        ConnectorClient connector = new ConnectorClient( new Uri( message.ServiceUrl ) );
        Activity replyMessage = message.CreateReply( "Personal data has been deleted." );

        return replyMessage;
      } else if ( message.Type == ActivityTypes.ConversationUpdate ) {
        // Handle conversation state changes, like members being added and removed
        // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
        // Not available in all channels
      } else if ( message.Type == ActivityTypes.ContactRelationUpdate ) {
        // Handle add/remove from contact lists
        // Activity.From + Activity.Action represent what happened
      } else if ( message.Type == ActivityTypes.Typing ) {
        // Handle knowing tha the user is typing
      } else if ( message.Type == ActivityTypes.Ping ) {
      }

      return null;
    }
  }
}
