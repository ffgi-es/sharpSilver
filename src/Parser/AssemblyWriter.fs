module SharpSilver.AssemblyWriter

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
    | _ -> ""