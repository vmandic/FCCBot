module Hannibal.Slack

open SlackConnector
open SlackConnector.Models
open SlackAPI
open ProcessTypes

type SlackMessageWithContext =
  | RTMMessage of SlackMessage
  | Incomming of ActionResponse

let slackRTM (token: string) f (inbox: SlackMessageWithContext Agent) =
  let connector = new SlackConnector.SlackConnector()
  let connection = connector.Connect( token ) |> Async.AwaitTask |> Async.RunSynchronously
  
  connection.add_OnMessageReceived( fun x -> inbox <! (RTMMessage x) ; System.Threading.Tasks.Task.CompletedTask; )

  let rec loop () = async {
    let! msg = inbox.Receive()
    
    printfn "MSG RECEIVED: %A" msg
    
    f (msg, connection)

    return! loop ()
  }
  loop ()

let startSlackRTM token f = spawnRestart <| slackRTM token f
