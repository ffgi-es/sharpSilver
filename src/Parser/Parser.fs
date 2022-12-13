module SharpSilver.Parser

open FParsec

open AST

type PResult = 
    | Success of Program
    | Failure of string

let (.>->.) a b = a .>> spaces .>>. b
let (.>->) a b = a .>> spaces .>> b
let (>->.) a b = a .>> spaces >>. b

type Unit = Unit

let parseEquate = pchar '='
let parseUnit = pstring "()"
let parseName = regex "[a-z0-9]+"
let parseReturn = pstring "=>"
let parseTypeName = regex "[A-z]+"
let parseFunctionLine = regex @"-{2,}>"
let parseEntryFunctionLine = regex @"={2,}>"
let parseInt = pint64
let parseFunctionEnd = pchar '.'

let parseType = [parseTypeName; parseUnit] |> choice

let parseFunctionName = parseName .>> pchar ':'
let parseFunctionReference = pchar ':' >>. parseName

let (.>->?) a b = a .>>? spaces .>>? b

let parseFunctionDeclaration = parseFunctionName .>> parseEquate

let parseFunctionTypes =
    many1 (spaces >>. parseType .>->? parseReturn) .>->. parseType

let parseFunctionSignature =
    parseFunctionDeclaration .>->. parseFunctionTypes
    |>> fun (name, (_, returnType)) -> {Name=name; ReturnType=returnType; Parameters=[]}

let parseIntReturn = parseInt |>> ReturnValue
//let parseFunctionCall = parseFunctionReference |>> FunctionCall

let parseFunctionCall =
    sepEndBy parseInt spaces .>->.
    parseFunctionReference .>->.
    sepEndBy parseInt spaces
    |>> fun ((i,name), j) -> FunctionCall {Function=name; Inputs= i@j } 

let parseExpression = (attempt parseFunctionCall) <|> parseIntReturn

let parseBody = parseUnit .>-> parseReturn >->. parseExpression

let parseFunction lineParser = 
    spaces >>.
    parseFunctionSignature .>->
    lineParser .>->.
    parseBody .>>
    parseFunctionEnd
    |>> fun (sign, expr) -> {Signature=sign; Body=expr}

let parseProgram =
    pipe2
        (many (attempt (parseFunction parseFunctionLine)))
        (parseFunction parseEntryFunctionLine)
        (fun funcs func ->
            {
                EntryPoint = func
                Functions =
                    List.map (fun f -> (f.Signature.Name, f)) funcs
                    |> dict
            }
        )

let parseFromString source =
    runParserOnString parseProgram () "function definition" source
    |> function
        | ParserResult.Success (result, _, _) -> Success result
        | ParserResult.Failure (error, _, _) -> Failure error
