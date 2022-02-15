// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open MinimalCover.Domain
open MinimalCover.Infrastructure

let testTextParser () = 
  let parserOptions: Parsers.Text.ParserOptions = 
    { AttributeSeparator = ","; FdSeparator = ";"; LeftRightSeparator = "-->" }
  
  try
    let fds = Parsers.Text.Parse parserOptions "a,b,c --> x,y; x,y --> a"
    printfn "Parsed from text parser: %A" fds
  with 
    | :? Exceptions.ParserException as ex -> printf "%A" ex.Message
    | _ -> reraise ()

let testJsonParser () =
  try
    let fds = Parsers.Json.Parse """ [ { "left": ["a"], "right": ["b"] } ] """
    printfn "Parsed from json parser: %A" fds
  with 
    | :? Exceptions.ParserException as ex -> printf "%A" ex.Message
    | _ -> reraise ()

[<EntryPoint>]
let main argv =
    //let fd1 = FunctionalDependency.T(set ["x"; "z"], set ["y"])
    //let fd2 = FunctionalDependency.T(set ["x"; "z"; "a"], set ["y"])
    testTextParser ()
    testJsonParser ()
    0

