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
        | Success result -> Assert.True(true)
        | Failure error -> error |> should equal "")