module AssemblyBuilderTests

open System
open Xunit
open FsCheck.Xunit
open FsUnit.Xunit

open SharpSilver.AST
open SharpSilver.AssemblyBuilder

open SharedUtilities

let simpleReturnProgramAssembly (name:string) (exit:int) =
    [
        "SECTION .text"
        ""
        $"_{name}:"
        $"   mov rdi, {exit}"
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
    |> should equal (simpleReturnProgramAssembly "main" 3)

[<Property>]
let ``Should map funciton name and return value`` (FunctionName name) (exit:int) =
    {
        EntryPoint = {
            Signature = {Name=name; Parameters=[]; ReturnType="INT"}
            Body = ReturnValue exit 
        }
        Functions = dict []
    }
    |> buildAssembly
    |> fun result -> result .=. simpleReturnProgramAssembly name exit

let simpleAdditionProgram (name:string) (a:int) (b:int) =
    [
        "SECTION .text"
        ""
        $"_{name}:"
        $"   mov rax, {a}"
        $"   add rax, {b}"
        $"   mov rdi, rax"
        "   mov rax, 60 ;sys_exit"
        "   syscall"
    ]
    |> String.concat Environment.NewLine

[<Property>]
let ``Should map addition expression`` (FunctionName name) (a:int) (b:int) =
    {
        EntryPoint = {
            Signature = {Name=name; Parameters=[]; ReturnType="INT"}
            Body = FunctionCall {Function="+"; Inputs=[a;b]} 
        }
        Functions = dict []
    }
    |> buildAssembly
    |> fun result -> result .=. simpleAdditionProgram name a b
