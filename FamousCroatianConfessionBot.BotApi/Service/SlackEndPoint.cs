using FamousCroatianConfessionBot.Bot;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using Microsoft.ProjectOxford.Emotion;
using Newtonsoft.Json;
using SlackAPI;
using SlackConnector;
using SlackConnector.Models;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Hannibal.Slack;

namespace FamousCroatianConfessionBot.Service {

  public class SlackEndPoint {

    internal static string SlackBotToken = System.Web.Configuration.WebConfigurationManager.AppSettings["SlackBotToken"];
    internal static string SlackVerifyToken = System.Web.Configuration.WebConfigurationManager.AppSettings["SlackVerifyToken"];

    internal static FSharpMailboxProcessor<SlackMessageWithContext> SlackIncommingMailbox = null;

    public static void StartSlackClient() {
      // Start Slack client
      SlackIncommingMailbox = startSlackRTM( SlackBotToken, FuncConvert.ToFSharpFunc<Tuple<SlackMessageWithContext, ISlackConnection>>( msg => {
        var x = msg.Item1;
        var conn = msg.Item2;

        if ( x.IsIncomming ) {
          var m = x as SlackMessageWithContext.Incomming;
          var val = m.Item;
          var r = val.ActionResults[0];
          var selVal = !string.IsNullOrWhiteSpace( r.Value )
          ? r.Value
          : r.SelectedOptions.Any() ? r.SelectedOptions[0].Value : "";

          // Reply
          var res = new BotMessage {
            //Text = $"```\n  {JsonConvert.SerializeObject(val)} \n```",
            Text = $"Selected: {selVal}",
            ChatHub = new SlackChatHub { Id = val.Channel.Id }
          };
          conn.Say( res );

        } else if ( x.IsRTMMessage ) {
          var m = x as SlackMessageWithContext.RTMMessage;
          var val = m.Item;
          var d = JsonConvert.DeserializeObject<Message>( val.RawData );
          if ( d.file != null ) {
            // Process file (image)
            var imgStream = (WebRequest.Create( d.file.url_private_download ).GetResponseAsync()).Result.GetResponseStream();
            var text = FccBot.RecognizeEmotionsFromPortraitImage( imgStream ).Result;
            var res = new BotMessage {
              ChatHub = val.ChatHub,
              Text = text
            };
          } else {
            // Process text
            var res = GetReplay( val );
            conn.Say( res );
          }
        }
      } ) );
    }

    static BotMessage GetReplay( SlackMessage msg ) {

      var a = new SlackAttachmentAction {
        Type = "button",
        Style = SlackAttachmentActionStyle.Danger,
        Name = "My action name",
        Text = "My action text",
        Value = "42"
      };

      var sel = new SelectAction {
        Name = "My action name",
        Text = "My action text",
        Type = "select",
        Options = new[] {
          new Option { Text = "Option one", Value = "1" },
          new Option { Text = "Option two", Value = "2" },
        },
        //DataSource = SelectAction.DS_USERS,
      };

      var att = new SlackAttachment {
        CallbackId = "my_msg",
        Text = "My att",
        Title = "My title",
        Actions = new[] { sel, a }
      };

      return new BotMessage {
        //Text = $"RTM message from: {msg.User.Name}\n  ```\n  {JsonConvert.SerializeObject(msg)} \n  ```",
        Text = $"Hej! {msg.Text}",
        ChatHub = msg.ChatHub,
        Attachments = new[] { att }
      };
    }
  }
}
