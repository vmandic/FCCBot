using System;

namespace SlackAPI {

  // https://github.com/Inumedia/SlackAPI/blob/4ab98f7be652b65049b59b3cb30874c714fa4d34/SlackAPI/File.cs

  public class Message {
    public string type { get; set; }
    public string subtype { get; set; }
    public File file { get; set; }

    public const string TYPE_MESSAGE = "TYPE_MESSAGE";
    public const string SUBTYPE_FILE_SHARE = "SUBTYPE_FILE_SHARE";
  }
}
