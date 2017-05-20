using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
  }
}