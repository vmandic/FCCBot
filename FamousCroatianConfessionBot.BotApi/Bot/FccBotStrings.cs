namespace FamousCroatianConfessionBot.Bot
{
  public static class FccBotStrings
  {
    public const string UNSUPPORTED_ATTACHMENT_TYPE =
      "I am sorry but I do not understand that thing... I mean the file type that you sent me. Try sending me an image instead please? :-)";

    public const string IMAGE_OVER_3MB =
      "Hey, that is a too big image for my small processor, please send another image but this time under 3MB of size.";

    public const string WHOOPS_STH_WENT_WRONG_HERES_THE_ERROR =
      "Whoooops! :-( Something went awfully wrong... Here is the detailed error message: {0}";

    public const string I_CAN_HANDLE_ONLY_ONE_ATT
      = "Hey! I can handle only one attachment and it must an image. Thanks.";

    public const string HELLO_MSG_AND_ASK_FOR_NAME
      = "Hey! What's up doc?\n\rWhat's your name? :)";

    public const string HEY_THERE_USER_SPEAK_UP
      = "Hey there {0}! Speak up... and I will probably repeat what you said.";

    public const string REPEAT_AND_ASK_FOR_SELFIE
      = "{0}, I'm repeating after you: \"{1}\"\n\rIf you'd like to see something smart... send me selfie photo! :)";

    public const string YOU_SAID_NOTHING_PLEASE_SPEAK_UP
      = "You said nothing! Please speak up something else then pure white space... :p\n\r\n\rWhat was that you wanted to say again?";

    public const string SHOW_HELP
      = "Hi. I am or was supposed to be Zdravko!\n\r\n\r\n\rI was made by three guys for fun and learning about building bots. :) My idea was to help you confess your daily programmer sins or any other sins as you wish but I stayed at a very rudimental intelligence level of just saying hello to you and repeating what you said. One sort of a smart thing I can do is tell you your emotions from your selfie picture you can send me. Ouh and yes, I WILL NOT SAVE ANY DATA YOU SEND ME (on my side of the implementation...), but please do not share sensitive info with me.\n\r\n\r\n\rThanks for chatting! :)";

    public const string SHOW_COMMANDS
      = "I'm not the smartest bot... but I'll recognize these key words:\n\r\n\r\n\rhelp - prints the about and help text\n\rcommands - prints these commands\n\rgood bye / bye - wipes out your conversation and tells you bye bye\n\rhi / hello - I'll greet you back :)";

    public const string GOOD_BYE
      = "Already going away? OK... this was fun, I'm cleaning up our conversation. Thanks and BB! :)";

    public const string HI_BACK
      = "...and a wonderful big HI to you too! :)";

    public const string HELLO_BACK
      = "...and a hello, howdy, what's up doc to you! :)";
  }
}