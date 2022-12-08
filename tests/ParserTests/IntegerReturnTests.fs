module IntegerReturnTests

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
        $"{name}:= () => INT"
        "==============>"
        $"() => {result}."
    ]
    |> String.concat Environment.NewLine

let testParsing name result property =
    generateIntegerReturn name result
    |> parseFromString
    |> property

let testSuccessfulParsing name result property =
    generateIntegerReturn name result
    |> parseFromString
    |> function
        | Success result -> property result
        | Failure error ->
            printf "Error produced:\n%s" error
            false


type functionName =
    static member Name() =
        let regex = new Regex(@"\A[a-z][a-z0-9]*\z")
        Arb.Default.String()
        |> Arb.filter (fun name -> 
            not(String.IsNullOrEmpty(name)) &&
            regex.Match(name).Success)

[<Fact>]
let ``Can parse a string`` () =
    testParsing "main" 2 (function
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
    |> testSuccessfulParsing "main" a

[<Property(Arbitrary=[|typeof<functionName>|])>]
let ``Should parse function name`` (a:string) =
    fun result -> result.EntryPoint.Signature.Name = a.Trim()
    |> testSuccessfulParsing a 2