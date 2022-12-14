module ArithmeticTests

open System
open Xunit

open FsCheck.Xunit
open FsUnit.Xunit

open SharpSilver.AST
open SharpSilver.Parser

open SharedUtilities

let functionCall (funcName:string) (input1:int) (input2:int) =
    [
        $"main:= () => INT"
        "=================>"
        $"() => {input1} :{funcName} {input2}."
    ]
    |> String.concat Environment.NewLine

let functionCallString (call:string) =
    [
        $"main:= () => INT"
        "=================>"
        $"() => {call}."
    ]
    |> String.concat Environment.NewLine

[<Fact>]
let ``Should parse a function call with arguments`` () =
    testParsing
        (functionCall "add" 2 3)
        (function
        | Success result ->
            result.EntryPoint.Body |> should equal (FunctionCall {Function="add"; Inputs=[2; 3]})
        | Failure error -> error |> should equal "")

[<Property>]
let ``Should parse function call with correct arguments`` (FunctionName fname) (input1:int) (input2:int) =
    testSuccessfulParsing
        (functionCall fname input1 input2)
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
        (functionCallString call)