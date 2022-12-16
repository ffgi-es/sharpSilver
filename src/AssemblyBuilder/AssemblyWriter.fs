module SharpSilver.AssemblyBuilder

open System

open SharpSilver.AST

let buildAssembly program =
    match program.EntryPoint.Body with
    | ReturnValue value ->
        [
            "SECTION .text"
            ""
            $"_{program.EntryPoint.Signature.Name}:"
            $"   mov rdi, {value}"
            "   mov rax, 60 ;sys_exit"
            "   syscall"
        ]
        |> String.concat Environment.NewLine
    | FunctionCall {Function=name; Inputs=inputs} ->
        let a = inputs.[0]
        let b = inputs.[1]
        [
            "SECTION .text"
            ""
            $"_{program.EntryPoint.Signature.Name}:"
            $"   mov rax, {a}"
            $"   add rax, {b}"
            $"   mov rdi, rax"
            "   mov rax, 60 ;sys_exit"
            "   syscall"
        ]
        |> String.concat Environment.NewLine