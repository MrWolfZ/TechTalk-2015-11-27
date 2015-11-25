module Boundary

open System

type Agent<'T> = MailboxProcessor<'T>

type Message<'TRequest, 'TResponse> = Message of 'TRequest * AsyncReplyChannel<'TResponse>

type Response<'T> =
    | Response of 'T
    | Error

type CircuitBreaker<'TRequest, 'TResponse> = Agent<Message<'TRequest, Response<'TResponse>>>

let replyAgentOf2Async f initial = Agent.Start <| fun inbox ->
    let rec loop state = async {
        let! Message (msg, replyChannel) = inbox.Receive()
        let! updated, response = f msg state
        replyChannel.Reply response
        return! loop updated
    }
    loop initial

let replyAgentOf2 f initial = replyAgentOf2Async (fun msg state -> async { return f msg state }) initial

type CircuitState = 
    | Open 
    | Closed of DateTime

type CircuitBreakerState = {
    CircuitState: CircuitState
    ErrorCount: int
}

let handleOpen f msg = async {
    try
        let! response = f msg
        return Response response
    with
    | _ -> return Error }

let handleErrorInOpen state limit =
    let newCount = state.ErrorCount + 1
    { state with
        CircuitState = if newCount < limit then Open else Closed DateTime.Now
        ErrorCount = newCount }

let handleClosed f msg time retryAfter = async {
    if DateTime.Now - time < TimeSpan.FromSeconds retryAfter
    then return Error
    else 
        let! response = handleOpen f msg
        match response with
        | Response r -> return Response r
        | Error -> return Error }

let initialState = { CircuitState = Open; ErrorCount = 0 }

let createFn (agent: CircuitBreaker<_, _>) param = async {
    let! response = agent.PostAndAsyncReply (fun rc -> Message (param, rc))
    match response with
    | Response r -> return r
    | Error -> return failwith "Error" }

let createCircuitBreaker f limit retryAfter =
    let loop msg state = async {
        match state.CircuitState with
        | Open -> 
            let! result = handleOpen f msg
            match result with
            | Response r -> return state, Response r
            | Error -> return handleErrorInOpen state limit, Error
        | Closed time -> 
            let! result = handleClosed f msg time retryAfter
            match result with
            | Response r -> return initialState, Response r
            | Error -> return state, Error }

    let agent = replyAgentOf2Async loop initialState

    createFn agent
