module SharpSilver.AssemblyBuilder

open System

open SharpSilver.AST

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
        [
            sprintf "SECTION .text"
            sprintf ""
            sprintf "_%s:" program.EntryPoint.Signature.Name
            sprintf "    mov rax, %d" a
            sprintf "    add rax, %d" b
            sprintf "    mov rdi, rax"
            sprintf "    mov rax, 60 ;sys_exit"
            sprintf "    syscall"
        ]
        |> String.concat Environment.NewLine