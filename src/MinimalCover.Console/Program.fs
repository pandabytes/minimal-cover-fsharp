// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open MinimalCover.Domain
open MinimalCover.Infrastructure

[<EntryPoint>]
let main argv =
    //let fd1 = FunctionalDependency.T(set ["x"; "z"], set ["y"])
    //let fd2 = FunctionalDependency.T(set ["x"; "z"; "a"], set ["y"])

    let parserOptions: Parsers.Text.ParserOptions = 
      { AttributeSeparator = ","; FdSeparator = ";"; LeftRightSeparator = "-->" }
    
    try
      let fds = Parsers.Text.Parse parserOptions "a,b,c --> x,y; x,y --> a"
      printfn "%A" (fds.Add (FunctionalDependency.T(set ["x"; "y"], set ["ab"])))
    with 
      | :? Exceptions.ParserException as ex -> printf "%A" ex.InnerException

    0