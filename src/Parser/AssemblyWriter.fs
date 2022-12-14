module SharpSilver.AssemblyWriter

open System

open SharpSilver.AST

let buildAssembly program =
    [
        "SECTION .text"
        ""
        "_main:"
        "   mov rdi, 3"
        "   mov rax, 60 ;sys_exit"
        "   syscall"
    ]
    |> String.concat Environment.NewLine