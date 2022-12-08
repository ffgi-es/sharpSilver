module SharpSilver.AST

type Parameter = {Type: string; Name: string}

type FunctionSignature = {Name: string; Parameters: Parameter list; ReturnType: string}

type Expression = 
    | ReturnValue of int64

type Function = {Signature: FunctionSignature; Body: Expression}
