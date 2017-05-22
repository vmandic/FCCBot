using FamousCroatianConfessionBot.Service;
using Newtonsoft.Json;
using SlackAPI;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace FamousCroatianConfessionBot {

  [AllowAnonymous]
  public class SlackCallbackController : ApiController {

    [HttpPost]
    public HttpResponseMessage Incomming( FormDataCollection form ) {
      ActionResponse response;
      try {
        response = JsonConvert.DeserializeObject<ActionResponse>( form["payload"] );
      } catch ( Exception ex ) {
        return Request.CreateResponse( HttpStatusCode.Forbidden, ex.GetBaseException().Message );
      }

      if ( response == null )
        return Request.CreateResponse( HttpStatusCode.NotAcceptable );

      if ( response.Token != SlackEndPoint.SlackVerifyToken )
        return Request.CreateResponse( HttpStatusCode.Forbidden );

      // Forward incomming response
      SlackEndPoint.SlackIncommingMailbox.Post( Hannibal.Slack.SlackMessageWithContext.NewIncomming( response ) );

      return Request.CreateResponse( HttpStatusCode.OK );
    }

  }
}
