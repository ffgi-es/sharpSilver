module FunctionTests

open System
open Xunit

open FsCheck.Xunit
open FsUnit.Xunit

open SharpSilver.AST
open SharpSilver.Parser

open SharedUtilities

let functionCall0 (funcName:string) (result:int) =
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
    testParsing (functionCall0 "other" 3) (function
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
    |> testSuccessfulParsing (functionCall0 funcName 3)

let functionCall2 (funcName:string) (input1:int) (input2:int) =
    [
        $"main:= () => INT"
        "=================>"
        $"() => {input1} :{funcName} {input2}."
    ]
    |> String.concat Environment.NewLine

let functionCall2String (call:string) =
    [
        $"main:= () => INT"
        "=================>"
        $"() => {call}."
    ]
    |> String.concat Environment.NewLine

[<Fact>]
let ``Should parse a function call with arguments`` () =
    testParsing
        (functionCall2 "add" 2 3)
        (function
        | Success result ->
            result.EntryPoint.Body |> should equal (FunctionCall {Function="add"; Inputs=[2; 3]})
        | Failure error -> error |> should equal "")

[<Property>]
let ``Should parse function call with correct arguments`` (FunctionName fname) (input1:int) (input2:int) =
    testSuccessfulParsing
        (functionCall2 fname input1 input2)
        (fun result -> result.EntryPoint.Body = (FunctionCall {Function=fname; Inputs=[input1; input2]}))

[<Theory>]
[<InlineData("4 :add 5")>]
[<InlineData(":add 4 5")>]
[<InlineData("4 5 :add")>]
let ``Should parse arguments regardless of function position`` (call:string) =
    function
    | Success result ->
        result.EntryPoint.Body |> should equal (FunctionCall {Function="add"; Inputs=[4; 5]})
    | Failure error -> error |> should equal ""
    |> testParsing
        (functionCall2String call)