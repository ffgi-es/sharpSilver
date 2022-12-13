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

[<Fact>]
let ``Should parse a function call with parameters`` () =
    testParsing
        (functionCall "add" 2 3)
        (function
        | Success result ->
            result.EntryPoint.Body |> should equal (FunctionCall {Function="add"; Inputs=[2; 3]})
        | Failure error -> error |> should equal "")

[<Property>]
let ``Should parse function call with correct parameters`` (FunctionName fname) (input1:int) (input2:int) =
    testSuccessfulParsing
        (functionCall fname input1 input2)
        (fun result -> result.EntryPoint.Body = (FunctionCall {Function=fname; Inputs=[input1; input2]}))