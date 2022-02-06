// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open MinimalCover.Domain

type A = int * int

type Product (code:string, price:float) =
   let isFree = price=0.0
   new (code) = Product(code,0.0)
   new (price) = Product("x",price)

   member this.Code = code
   member this.IsFree = isFree

[<EntryPoint>]
let main argv =
    let fd1 = FunctionalDependency.T(set ["x"; "z"], set ["y"])
    let fd2 = FunctionalDependency.T(set ["x"; "z"; "a"], set ["y"])
    let equal = FunctionalDependency.AreEqual fd1 fd2
    printfn "Equal? %b" equal
    0