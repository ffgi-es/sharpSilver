module AssemblyBuilderTests

open System
open Xunit
open FsCheck.Xunit
open FsUnit.Xunit

open SharpSilver.AST
open SharpSilver.AssemblyWriter

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