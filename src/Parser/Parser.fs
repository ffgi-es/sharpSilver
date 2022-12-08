module SharpSilver.Parser

open FParsec

open AST

type PResult = 
    | Success of Function
    | Failure of string

let skipWhiteSpace = skipMany (satisfy System.Char.IsWhiteSpace)

type Unit = Unit

let parseEquate = pchar '=' .>> skipWhiteSpace
let parseUnit = pstring "()" .>> skipWhiteSpace
let parseName = regex "[a-z0-9]+"
let parseReturn = pstring "=>" .>> skipWhiteSpace
let parseTypeName = regex "[A-z]+" .>> skipWhiteSpace
let parseFunctionLine = regex "={2,}>" .>> skipWhiteSpace
let parseInt = pint64 .>> skipWhiteSpace

let parseType = [parseTypeName; parseUnit] |> choice

let parseFunctionName = parseName .>> pchar ':' .>> skipWhiteSpace

let parseFunctionTypes =
    many1 (parseType .>>? parseReturn) .>>. parseType

let parseFunctionDeclaration = parseFunctionName .>> parseEquate

let parseFunctionSignature =
    pipe2
        parseFunctionDeclaration
        parseFunctionTypes
        (fun name (_, returnType) -> {Name=name; ReturnType=returnType; Parameters=[]})
    .>> parseFunctionLine
    .>> skipWhiteSpace

let parseExpression = parseInt |>> ReturnValue

let parseBody = parseUnit .>> parseReturn >>. parseExpression

let parseFunction = 
    skipWhiteSpace >>.
    pipe2
        parseFunctionSignature
        parseBody
        (fun signature expr -> {Signature=signature; Body=expr})

let parseFromString source =
    runParserOnString parseFunction () "function definition" source
    |> function
        | ParserResult.Success (result, _, _) -> Success result
        | ParserResult.Failure (error, _, _) -> Failure error
