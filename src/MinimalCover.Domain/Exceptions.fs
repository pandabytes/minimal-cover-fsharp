namespace MinimalCover.Domain

open System

module Exceptions = 
  type ParserException(message: string, ?innerException: Exception) =
    inherit Exception(message)

    member this.HasInnerException = not innerException.IsNone

    member this.InnerException = 
      if (not this.HasInnerException) then
        failwith ($"Cannot acces {nameof(this.InnerException)} because it is null. Please use " +
                  $"{nameof(this.HasInnerException)} to check if it exists before accesing it.")
      innerException.Value
