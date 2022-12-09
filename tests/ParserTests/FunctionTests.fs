module FunctionTests

open System
open System.Text.RegularExpressions
open Xunit

open FsCheck
open FsCheck.Xunit
open FsUnit.Xunit

open SharpSilver.AST
open SharpSilver.Parser

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

let testParsing name result property =
    functionCall name result
    |> parseFromString
    |> property

let testSuccessfulParsing name result property =
    functionCall name result
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
let ``Should parse function call`` () =
    testParsing "other" 3 (function
        | Success result ->
            result.EntryPoint.Signature.Name |> should equal "main"
            result.EntryPoint.Body |> should equal (FunctionCall "other")
            result.Functions |> should haveCount 1
            result.Functions.Keys |> should contain "other"
            result.Functions.["other"].Signature.Name |> should equal "other"
            result.Functions.["other"].Body |> should equal (ReturnValue 3)
        | Failure error -> error |> should equal "")

[<Property(Arbitrary=[|typeof<functionName>|])>]
let ``Should parse function name and call`` funcName =
    (fun result ->
        result.EntryPoint.Body = FunctionCall funcName &&
        result.Functions.Keys.Contains(funcName) &&
        result.Functions.[funcName].Signature.Name = funcName)
    |> testSuccessfulParsing funcName 3