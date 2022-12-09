module IntegerReturnTests

open System
open Xunit

open FsCheck.Xunit
open FsUnit.Xunit

open SharpSilver.AST
open SharpSilver.Parser

open SharedUtilities

let generateIntegerReturn (name:string) (result:int) =
    [
        $"{name}:= () => INT"
        "==============>"
        $"() => {result}."
    ]
    |> String.concat Environment.NewLine

[<Fact>]
let ``Can parse a string`` () =
    testParsing (generateIntegerReturn "main" 2) (function
        | Success result ->
            result.EntryPoint.Signature.Name |> should equal "main"
            result.EntryPoint.Signature.Parameters |> should haveLength 0
            result.EntryPoint.Signature.ReturnType |> should equal "INT"
            result.EntryPoint.Body |> should equal (ReturnValue 2)
        | Failure error ->
            printfn "Error received:\n%s" error
            error |> should equal "")

[<Property>]
let ``Should parse integer in return statement`` (a:int) =
    fun result -> result.EntryPoint.Body = ReturnValue a
    |> testSuccessfulParsing (generateIntegerReturn "main" a)

[<Property>]
let ``Should parse function name`` (FunctionName a) =
    fun result -> result.EntryPoint.Signature.Name = a
    |> testSuccessfulParsing (generateIntegerReturn a 2)