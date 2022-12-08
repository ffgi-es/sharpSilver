module SharpSilver.Parser

open System.Text.RegularExpressions
open FParsec

open AST

type PResult = 
    | Success of Function
    | Failure of string

let skipWhiteSpace = skipMany (satisfy System.Char.IsWhiteSpace)

let parseFunctionName = regex "[a-z0-9]+" .>> skipWhiteSpace
let parseReturn = pstring "=>" .>> skipWhiteSpace
let parseType = regex "[A-z]+" .>> skipWhiteSpace
let parseFunctionLine = regex "={3,}" .>> skipWhiteSpace
let parseInt = pint64 .>> skipWhiteSpace

let parseFunctionSignature =
    pipe2
        (parseFunctionName .>> parseReturn)
        parseType
        (fun name returnType -> {Name=name; ReturnType=returnType; Parameters=[]})
    .>> parseFunctionLine
    .>> skipWhiteSpace

let parseExpression = parseInt |>> ReturnValue

let parseFunction = 
    skipWhiteSpace >>.
    pipe2
        (parseFunctionSignature .>> parseFunctionName .>> parseReturn)
        parseExpression
        (fun signature expr -> {Signature=signature; Body=expr})

let parseFromString source =
    runParserOnString parseFunction () "function definition" source
    |> function
        | ParserResult.Success (result, _, _) -> Success result
        | ParserResult.Failure (error, _, _) -> Failure error
