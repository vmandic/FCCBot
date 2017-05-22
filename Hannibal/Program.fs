open System.Configuration
open SlackConnector.Models
open Hannibal.Slack

[<EntryPoint>]
let main argv = 
    printfn "%A" argv

    let slackToken = ConfigurationManager.AppSettings.["SlackBotToken"];

    // Start Slack client
    let _ = startSlackRTM slackToken <| fun (x, conn) ->
      match x with
      | Incomming msg -> ()
      | RTMMessage msg ->
        // Reply
        let res =
          BotMessage (
            ChatHub = msg.ChatHub,
            Text = "Hej RTM! " + msg.Text
          )
        conn.Say( res ) |> ignore
    
    System.Console.ReadKey() |> ignore

    0
