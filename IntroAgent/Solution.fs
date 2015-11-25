module Solution

type Agent<'T> = MailboxProcessor<'T>

type Message<'TRequest, 'TResponse> = Message of 'TRequest * AsyncReplyChannel<'TResponse>

let agentOfAsync f = Agent.Start <| fun inbox ->
    let rec loop () = async {
        let! msg = inbox.Receive()
        do! f msg
        return! loop()
    }
    loop()

let agentOf f = agentOfAsync (fun msg -> async { f msg })

let agentOf2Async f initial = Agent.Start <| fun inbox ->
    let rec loop state = async {
        let! msg = inbox.Receive()
        let! updated = f msg state
        return! loop updated
    }
    loop initial

let agentOf2 f initial = agentOf2Async (fun msg state -> async { return f msg state }) initial

let replyAgentOf2Async f initial = Agent.Start <| fun inbox ->
    let rec loop state = async {
        let! Message (msg, replyChannel) = inbox.Receive()
        let! updated, response = f msg state
        replyChannel.Reply response
        return! loop updated
    }
    loop initial

let replyAgentOf2 f initial = replyAgentOf2Async (fun msg state -> async { return f msg state }) initial

let run () =
    // stateless
    let stateless = agentOf <| printfn "%s"
    stateless.Post "Hello World"

    // stateful (mutable)
    let statefulMutable = Agent.Start <| fun inbox ->
        async {
            let mutable count = 0
            while true do
                let! msg = inbox.Receive()
                printfn "%s" msg
                count <- count + 1
                printfn "Received %d message(s) so far" count
        }

    statefulMutable.Post "Increase dat count"
    
    // stateful (immutable)
    let statefulImmutable = 
        agentOf2 (fun m count ->
            printfn "%s" m
            printfn "Received %d message(s) so far" <| count + 1
            count + 1) 0

    statefulImmutable.Post "Immutability rulez"

    // request/response
    let requestResponse = 
        replyAgentOf2 (fun m count ->
            printfn "%s" m
            count + 1, count + 1) 0

    let response = requestResponse.PostAndReply (fun rc -> Message ("What's the count?", rc))
    printfn "Count is %d" response

    ()