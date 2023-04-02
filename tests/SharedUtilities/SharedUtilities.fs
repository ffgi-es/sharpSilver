module SharedUtilities

open System
open System.Text.RegularExpressions
open Fare
open FsCheck
open FsCheck.Xunit

open SharpSilver.Parser

let positiveChar c1 =
    if Char.IsWhiteSpace(c1)
    then if c1 = ' ' then '·' else c1
    else '-'

let DiffStrings (s1 : string) (s2 : string) =
   let s1', s2' = s1.PadRight(s2.Length), s2.PadRight(s1.Length)

   let d1, d2 =
      (s1', s2')
      ||> Seq.zip
      |> Seq.map (fun (c1, c2) -> if c1 = c2 then (positiveChar c1),(positiveChar c2) else c1, c2)
      |> Seq.fold (fun (d1, d2) (c1, c2) -> (sprintf "%s%c" d1 c1), (sprintf "%s%c" d2 c2) ) ("","")
   sprintf "A:\n%s\nB:\n%s" d1 d2

let (.=.) left right = left = right |@ (sprintf "\n%A\nshould equal---------------\n%A\ndiff:\n%s" left right (DiffStrings $"{left}" $"{right}"))

let matching pattern =
    Gen.sized (fun size ->
        let xeger = Xeger pattern
        let count = if size < 1 then 1 else size
        [ for i in 1..count -> xeger.Generate()]
        |> Gen.elements
        |> Gen.resize count)

type FunctionName = FunctionName of string with
    override this.ToString() =
        match this with
        | FunctionName name -> name

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