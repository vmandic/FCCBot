using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackConnector.Models;

namespace SlackAPI {

  // https://api.slack.com/docs/interactive-message-field-guide

  public class SelectAction : SlackAttachmentAction {
    public Option[] Options { get; set; }
    public string Confirm { get; set; }
    [JsonProperty( PropertyName = "data_source" )]
    public string DataSource { get; set; }
    public const string DS_USERS = "users";
    public const string DS_CHANNELS = "channels";
  }

  public class Option {
    public string Text { get; set; }
    public string Value { get; set; }
  }

  public class Confirm {
    public string Title { get; set; }
    public string Text { get; set; }
    [JsonProperty( PropertyName = "ok_text" )]
    public string OkText { get; set; }
    [JsonProperty( PropertyName = "dismiss_text" )]
    public string CancelText { get; set; }
  }

  public class ActionResponse {
    [JsonProperty( PropertyName = "callback_id" )]
    public string CallbackId { get; set; }
    public SlackTeam Team { get; set; }
    public IdName Channel { get; set; }
    public IdName User { get; set; }
    [JsonProperty( PropertyName = "attachment_id" )]
    public string AttachmentId { get; set; }
    public string Token { get; set; }
    [JsonProperty( PropertyName = "original_message" )]
    public JToken OriginalMessage { get; set; }
    [JsonProperty( PropertyName = "response_url" )]
    public string ResponseUrl { get; set; }
    [JsonProperty( PropertyName = "actions" )]
    public ActionResult[] ActionResults { get; set; }
  }

  public class SlackTeam {
    public string Id { get; set; }
    public string Domain { get; set; }
  }

  public class IdName {
    public string Id { get; set; }
    public string Name { get; set; }
  }

  public class ActionResult {
    public string Name { get; set; }
    [JsonProperty( PropertyName = "selected_options" )]
    public ActionResultValue[] SelectedOptions { get; set; }
    public string Value { get; set; }
  }

  public class ActionResultValue {
    public string Value { get; set; }
  }

}
