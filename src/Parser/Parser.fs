module SharpSilver.Parser

open FParsec

open AST

type PResult = 
    | Success of Program
    | Failure of string

let skipWhiteSpace = skipMany (satisfy System.Char.IsWhiteSpace)

let (.>->.) a b = a .>> skipWhiteSpace .>>. b
let (.>->) a b = a .>> skipWhiteSpace .>> b
let (>->.) a b = a .>> skipWhiteSpace >>. b

type Unit = Unit

let parseEquate = pchar '=' .>> skipWhiteSpace
let parseUnit = pstring "()" .>> skipWhiteSpace
let parseName = regex "[a-z0-9]+"
let parseReturn = pstring "=>" .>> skipWhiteSpace
let parseTypeName = regex "[A-z]+" .>> skipWhiteSpace
let parseFunctionLine = regex @"-{2,}>" .>> skipWhiteSpace
let parseEntryFunctionLine = regex @"={2,}>" .>> skipWhiteSpace
let parseInt = pint64 .>> skipWhiteSpace
let parseFunctionEnd = pchar '.'

let parseType = [parseTypeName; parseUnit] |> choice

let parseFunctionName = parseName .>> pchar ':' .>> skipWhiteSpace
let parseFunctionReference = pchar ':' >>. parseName .>> skipWhiteSpace

let parseFunctionTypes =
    many1 (parseType .>>? parseReturn) .>>. parseType

let parseFunctionDeclaration = parseFunctionName .>> parseEquate

let parseFunctionSignature lineParser =
    pipe2
        parseFunctionDeclaration
        parseFunctionTypes
        (fun name (_, returnType) -> {Name=name; ReturnType=returnType; Parameters=[]})
    .>> lineParser
    .>> skipWhiteSpace

let parseIntReturn = parseInt |>> ReturnValue
let parseFunctionCall = parseFunctionReference >>% ReturnValue 0

let parseExpression = parseIntReturn <|> parseFunctionCall

let parseBody = parseUnit .>> parseReturn >>. parseExpression

let parseFunction lineParser = 
    skipWhiteSpace >>.
    pipe2
        (parseFunctionSignature lineParser)
        parseBody
        (fun signature expr -> {Signature=signature; Body=expr})
    .>> parseFunctionEnd

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
