module SharpSilver.AST
open System.Collections.Generic

type Parameter = {Type: string; Name: string}

type FunctionSignature = {Name: string; Parameters: Parameter list; ReturnType: string}

type Expression = 
    | ReturnValue of int64

type Function = {Signature: FunctionSignature; Body: Expression}

type Program = {EntryPoint: Function; Functions: IDictionary<string, Function>}
