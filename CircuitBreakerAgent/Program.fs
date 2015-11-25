open Boundary
open Util
open System
open System.Threading

[<EntryPoint>]
let main argv = 
    let count = ref 0
    let unreliable x = async {
        if !count >= 5 
        then 
            count.Value <- 0
            return failwith "error"
        else
            count.Value <- !count + 1
            return x }

    let cb = createCircuitBreaker unreliable 2 5.0

    let rec loop count = async {
        if count < 100 then
            try
                let! result = cb count
                cprintf ConsoleColor.Green "Success"
                printfn " at count: %d" result
            with
            | _ -> 
                cprintf ConsoleColor.Red "Failure"
                printfn " at count: %d" count
            do! Async.Sleep 500
            return! loop <| count + 1 }
    
    let cts = new CancellationTokenSource()
    Async.Start (loop 0, cancellationToken = cts.Token)
    Console.ReadKey() |> ignore
    printfn "Cancelling..."
    cts.Cancel()

    0 // return an integer exit code
