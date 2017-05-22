module Hannibal.ProcessTypes

type 'a Agent = 'a MailboxProcessor

let (<!) (agent: _ Agent) msg = agent.Post msg

let (!>) msg (agent: _ Agent) = agent.Post msg

let spawn agent = Agent.Start agent

let spawnRestart agent =
  let rec loop f x = async {
    try return! f x
    with ex ->
      // Agent error. Restarting agent.
      printfn "Agent error: %A" <| ex.GetBaseException()
      do! Async.Sleep 2000
    return! loop f x
  }
  spawn <| loop agent

let defer time a = async {
    do! Async.Sleep time
    try do! a
    finally ()
  }

let schedulerAgent (inbox: _ Agent) =
  let rec loop () = async {
    let! sleep, f = inbox.Receive()
    Async.Start <| async {
      do! Async.Sleep sleep
      try f ()
      finally ()
    }
    return! loop ()
  }
  loop ()
