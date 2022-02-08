namespace MinimalCover.Infrastructure.Parsers

open System
open MinimalCover.Domain

module Text =
  type ParserOptions = { AttributeSeparator: string; FdSeparator: string; LeftRightSeparator: string }

  let private ValidateParserOptions (parserOptions: ParserOptions) =
    if (String.IsNullOrWhiteSpace(parserOptions.AttributeSeparator) ||
        String.IsNullOrWhiteSpace(parserOptions.FdSeparator) ||
        String.IsNullOrWhiteSpace(parserOptions.LeftRightSeparator)) then
      raise (ArgumentException("All separators must be non-empty and non-null strings"))
  
  let Parse (parserOptions: ParserOptions) (value: string) =
    ValidateParserOptions parserOptions

    let fdStrings = value.Split parserOptions.FdSeparator 
                      |> Array.filter (String.IsNullOrEmpty >> not)
                      |> Array.map (fun fd -> fd.Trim())

    let GetAtributesWithSeparator (value: string) =
      value.Split parserOptions.AttributeSeparator |> Array.map (fun attribute -> attribute.Trim())

    let fds = Array.map (fun (fd: string) -> 
                let fdTokens = fd.Split (parserOptions.LeftRightSeparator)
                if (fdTokens.Length <> 2) then
                  let message = $"LHS and RHS must be separated by \"{parserOptions.LeftRightSeparator}\"."
                  raise (Exceptions.ParserException message)

                let leftAttributeStr = fdTokens.[0];
                let rightAttributeStr = fdTokens.[1];
                if ( (String.IsNullOrWhiteSpace leftAttributeStr) || (String.IsNullOrWhiteSpace rightAttributeStr)) then
                  let message = $"LHS and RHS must not be empty. Invalid functional dependency \"{fd}\"."
                  raise (Exceptions.ParserException message)

                let leftAttributes = GetAtributesWithSeparator leftAttributeStr |> set
                let rightAttributes = GetAtributesWithSeparator rightAttributeStr |> set
                FunctionalDependency.T (leftAttributes, rightAttributes))

                fdStrings
    fds |> set
