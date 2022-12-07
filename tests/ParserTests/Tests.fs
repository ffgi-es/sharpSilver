module Tests

open System
open System.Text.RegularExpressions
open Xunit

open FsCheck
open FsCheck.Xunit
open FsUnit.Xunit

open SharpSilver.AST
open SharpSilver.Parser

let generateIntegerReturn (name:string) (result:int) =
    [
        $"{name} => INT"
        "=============="
        $"{name} => {result}."
    ]
    |> String.concat Environment.NewLine

let testParsing name result property =
    generateIntegerReturn name result
    |> parseFromString
    |> property

type functionName =
    static member Name() =
        let regex = new Regex("^[a-z][A-z0-9]*$")
        Arb.Default.String()
        |> Arb.filter (fun name -> 
            not(String.IsNullOrEmpty(name)) &&
            regex.Match(name).Success)

[<Fact>]
let ``Can parse a string`` () =
    testParsing "main" 2 (fun result ->
        result.Signature.Name |> should equal "main"
        result.Signature.Parameters |> should haveLength 0
        result.Signature.ReturnType |> should equal "INT"
        result.Body |> should equal (ReturnValue 2))

[<Property>]
let ``Should parse integer in return statement`` (a:int) =
    fun result -> result.Body = ReturnValue a
    |> testParsing "main" a

[<Property(Arbitrary=[|typeof<functionName>|])>]
let ``Should parse function name`` (a:string) =
    fun result -> result.Signature.Name = a
    |> testParsing a 2