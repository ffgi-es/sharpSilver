module ArithmeticTests

open System
open Xunit

open FsCheck.Xunit
open FsUnit.Xunit

open SharpSilver.AST
open SharpSilver.Parser

open SharedUtilities

let add (input1:int) (input2:int) =
    [
        $"main:= () => INT"
        "=================>"
        $"() => {input1} :+ {input2}."
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
let ``Should parse an addition expression`` () =
    testParsing
        (add 2 3)
        (function
        | Success result ->
            result.EntryPoint.Body |> should equal (FunctionCall {Function="+"; Inputs=[2; 3]})
        | Failure error -> error |> should equal "")

[<Property>]
let ``Should parse addition correct arguments`` (input1:int) (input2:int) =
    testSuccessfulParsing
        (add input1 input2)
        (fun result -> result.EntryPoint.Body = (FunctionCall {Function="+"; Inputs=[input1; input2]}))

[<Theory>]
[<InlineData("4 :+ 5")>]
[<InlineData(":+ 4 5")>]
[<InlineData("4 5 :+")>]
let ``Should parse arguments regardless of function position`` (call:string) =
    function
    | Success result ->
        result.EntryPoint.Body |> should equal (FunctionCall {Function="+"; Inputs=[4; 5]})
    | Failure error -> error |> should equal ""
    |> testParsing
        (functionCall2String call)