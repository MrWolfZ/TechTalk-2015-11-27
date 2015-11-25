module Template

type Agent<'T> = MailboxProcessor<'T>

type Message<'TRequest, 'TResponse> = Message of 'TRequest * AsyncReplyChannel<'TResponse>

let run () =
    // stateless

    // stateful (mutable)
    
    // stateful (immutable)

    // request/response

    ()

