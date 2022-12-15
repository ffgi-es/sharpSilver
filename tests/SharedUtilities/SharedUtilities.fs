module SharedUtilities

open System
open System.Text.RegularExpressions
open FsCheck
open FsCheck.Xunit

open SharpSilver.Parser

let (.=.) left right = left = right |@ sprintf "%A\nshould equal---------------\n%A" left right

type FunctionName = FunctionName of string with
    static member op_Explicit(FunctionName name) = name

let functionNameRegex = new Regex(@"\A[a-z][a-z0-9]*\z")

type ArbitraryModifiers =
    static member FunctionName() =
        Arb.Default.String()
        |> Arb.filter (fun name ->
            not(String.IsNullOrEmpty(name)) &&
            functionNameRegex.IsMatch(name))
        |> Arb.convert FunctionName string

[<assembly: Properties(Arbitrary= [|typeof<ArbitraryModifiers>|])>] do()

let testParsing code property =
    code
    |> parseFromString
    |> property

let testSuccessfulParsing code property =
    code
    |> parseFromString
    |> function
        | Success result -> property result
        | Failure error ->
            printf "Error produced:\n%s" error
            false