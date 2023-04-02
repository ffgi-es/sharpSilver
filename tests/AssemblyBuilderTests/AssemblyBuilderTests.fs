module AssemblyBuilderTests

open System
open Xunit
open FsCheck.Xunit
open FsUnit.Xunit

open SharpSilver.AST
open SharpSilver.AssemblyBuilder

open SharedUtilities

let buildAndCompare (program, assemblyCode) = (buildAssembly program) .=. assemblyCode

let simpleReturnProgramAssembly (name:string) (exit:int) =
    (
        {
            EntryPoint = {
                Signature = {Name=name; Parameters=[]; ReturnType="INT"}
                Body = ReturnValue exit
            }
            Functions = dict []
        },
        $"""
SECTION .text

_{name}:
    mov rdi, {exit}
    mov rax, 60 ;sys_exit
    syscall
""".Trim()
    )

[<Fact>]
let ``Should build simple return program`` () =
    simpleReturnProgramAssembly "main" 3 |> buildAndCompare

[<Property>]
let ``Should map function name and return value`` (FunctionName name) (exit:int) =
    simpleReturnProgramAssembly name exit |> buildAndCompare

let simpleAdditionProgram (name:string) (a:int) (b:int) =
    (
        {
            EntryPoint = {
                Signature = {Name=name; Parameters=[]; ReturnType="INT"}
                Body = FunctionCall {Function="+"; Inputs=[a;b]}
            }
            Functions = dict []
        },
        $"""
SECTION .text

_{name}:
    mov rax, {a}
    add rax, {b}
    mov rdi, rax
    mov rax, 60 ;sys_exit
    syscall
""".Trim()
    )

[<Property>]
let ``Should map addition expression`` (FunctionName name) (a:int) (b:int) =
    simpleAdditionProgram name a b |> buildAndCompare

let simpleSubtractionProgram (name:string) (a:int) (b:int) =
    (
        {
            EntryPoint = {
                Signature = {Name=name; Parameters=[]; ReturnType="INT"}
                Body = FunctionCall {Function="-"; Inputs=[a;b]}
            }
            Functions = dict []
        },
        $"""
SECTION .text

_{name}:
    mov rax, {a}
    sub rax, {b}
    mov rdi, rax
    mov rax, 60 ;sys_exit
    syscall
""".Trim()
    )

[<Property>]
let ``Should map subtraction expression`` (FunctionName name) (a:int) (b:int) =
    simpleSubtractionProgram name a b |> buildAndCompare

let simpleDivisionProgram (a:int) (b:int) =
    (
        {
            EntryPoint = {
                Signature = {Name="main"; Parameters=[]; ReturnType="INT"}
                Body = FunctionCall {Function="/"; Inputs=[a;b]}
            }
            Functions = dict []
        },
        $"""
SECTION .text

_main:
    mov rax, {a}
    mov rdi, {b}
    xor rdx, rdx
    idiv rdi
    mov rdi, rax
    mov rax, 60 ;sys_exit
    syscall
""".Trim()
    )

[<Property>]
let ``Should map division expression`` (a:int) (b:int) =
    simpleDivisionProgram a b |> buildAndCompare