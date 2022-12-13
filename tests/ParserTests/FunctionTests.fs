module FunctionTests

open System
open Xunit

open FsCheck.Xunit
open FsUnit.Xunit

open SharpSilver.AST
open SharpSilver.Parser

open SharedUtilities

let functionCall (funcName:string) (result:int) =
    [
        $"{funcName}:= () => INT"
        "---------------------->"
        $"() => {result}."
        ""
        $"main:= () => INT"
        "=================>"
        $"() => :{funcName}."
    ]
    |> String.concat Environment.NewLine

[<Fact>]
let ``Should parse function call`` () =
    testParsing (functionCall "other" 3) (function
        | Success result ->
            result.EntryPoint.Signature.Name |> should equal "main"
            result.EntryPoint.Body |> should equal (FunctionCall {Function="other"; Inputs=[]})
            result.Functions |> should haveCount 1
            result.Functions.Keys |> should contain "other"
            result.Functions.["other"].Signature.Name |> should equal "other"
            result.Functions.["other"].Body |> should equal (ReturnValue 3)
        | Failure error -> error |> should equal "")

[<Property>]
let ``Should parse function name and call`` (FunctionName funcName) =
    (fun result ->
        result.EntryPoint.Body = FunctionCall {Function=funcName; Inputs=[]} &&
        result.Functions.Keys.Contains(funcName) &&
        result.Functions.[funcName].Signature.Name = funcName)
    |> testSuccessfulParsing (functionCall funcName 3)