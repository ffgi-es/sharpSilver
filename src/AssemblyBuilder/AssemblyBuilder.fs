module SharpSilver.AssemblyBuilder

open System

open SharpSilver.AST

let AddSumCode name operation a b =
    [
        sprintf "SECTION .text"
        sprintf ""
        sprintf "_%s:" name
        sprintf "    mov rax, %d" a
        sprintf "    %s rax, %d" operation b
        sprintf "    mov rdi, rax"
        sprintf "    mov rax, 60 ;sys_exit"
        sprintf "    syscall"
    ]
    |> String.concat Environment.NewLine

let DivCode name a b =
    [
        sprintf "SECTION .text"
        sprintf ""
        sprintf "_%s:" name
        sprintf "    mov rax, %d" a
        sprintf "    mov rdi, %d" b
        sprintf "    xor rdx, rdx"
        sprintf "    idiv rdi"
        sprintf "    mov rdi, rax"
        sprintf "    mov rax, 60 ;sys_exit"
        sprintf "    syscall"
    ]
    |> String.concat Environment.NewLine

let buildAssembly program =
    match program.EntryPoint.Body with
    | ReturnValue value ->
        [
            sprintf "SECTION .text"
            sprintf ""
            sprintf "_%s:" program.EntryPoint.Signature.Name
            sprintf "    mov rdi, %d" value
            sprintf "    mov rax, 60 ;sys_exit"
            sprintf "    syscall"
        ]
        |> String.concat Environment.NewLine
    | FunctionCall {Function=name; Inputs=inputs} ->
        let a = inputs.[0]
        let b = inputs.[1]
        let operation = name |> function
            | "+" -> AddSumCode program.EntryPoint.Signature.Name "add" a b
            | "-" -> AddSumCode program.EntryPoint.Signature.Name "sub" a b
            | "/" -> DivCode program.EntryPoint.Signature.Name a b
            | _ -> "ARG"
        operation