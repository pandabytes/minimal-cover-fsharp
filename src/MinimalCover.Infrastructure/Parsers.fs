namespace MinimalCover.Infrastructure.Parsers

open System
open System.Collections.Generic
open MinimalCover.Domain
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Newtonsoft.Json.Schema

open FSharp.Data

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

module Json =
  type ParserSettings = { SchemaFilePath: string option }

  let private ValidateJson (schema: string) (jsonStr: string) =
    let schema = JSchema.Parse(schema);
    let jToken =
      try
        JToken.Parse(jsonStr)
      with
        | :? JsonReaderException as ex -> 
          let message = $"Fail to parse the given JSON string \"{jsonStr}\". " + 
                        "This string may not be in the correct JSON format."
          raise (Exceptions.ParserException (message, ex))

    let mutable validationErrors: IList<ValidationError> = upcast List<ValidationError>();
    jToken.IsValid (schema, &validationErrors) |> ignore

    if (validationErrors.Count > 0) then
      let errorMessage =
        Seq.fold
          (fun (prev: string) (current: ValidationError) ->
            $"{prev}{Environment.NewLine}{current.Message} "
            + $"Path: {current.Path}. "
            + $"Line number: {current.LineNumber}.")
          "Fail to validate JSON string."
          validationErrors

      raise (Exceptions.ParserException errorMessage)

    jToken
