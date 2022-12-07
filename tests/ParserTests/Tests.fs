module Tests

open System
open Xunit

open FsCheck
open FsCheck.Xunit
open FsUnit.Xunit

open SharpSilver.AST
open SharpSilver.Parser

[<Fact>]
let ``Can parse a string`` () =
    let input =
        [
            "main => INT"
            "==========="
            "main => 2."
        ]
        |> String.concat Environment.NewLine
    let result = parseFromString input
    result.Signature.Name |> should equal "main"
    result.Signature.Parameters |> should haveLength 0
    result.Signature.ReturnType |> should equal "INT"
    result.Body |> should equal (ReturnValue 2)

[<Property>]
let ``Should parse integer in return statement`` (a:int) =
    let input =
        [
            "main => INT"
            "==========="
            $"main => {a}."
        ]
        |> String.concat Environment.NewLine
    let result = parseFromString input
    result.Body = ReturnValue a
