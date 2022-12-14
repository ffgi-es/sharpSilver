module AssemblyBuilderTests

open System
open Xunit
open FsUnit.Xunit

open SharpSilver.AST
open SharpSilver.AssemblyWriter


let simpleReturnProgramAssembly =
    [
        "SECTION .text"
        ""
        "_main:"
        "   mov rdi, 3"
        "   mov rax, 60 ;sys_exit"
        "   syscall"
    ]
    |> String.concat Environment.NewLine

[<Fact>]
let ``Should build simple return program`` () =
    {
        EntryPoint = {
            Signature = {Name="main"; Parameters=[]; ReturnType="INT"}
            Body = ReturnValue 3
        }
        Functions = dict []
    }
    |> buildAssembly
    |> should equal simpleReturnProgramAssembly