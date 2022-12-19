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
        sprintf "SECTION .text"
        sprintf ""
        sprintf "_%s:" name
        sprintf "    mov rdi, %d" exit
        sprintf "    mov rax, 60 ;sys_exit"
        sprintf "    syscall"
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
let ``Should map function name and return value`` (FunctionName name) (exit:int) =
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
        sprintf "SECTION .text"
        sprintf ""
        sprintf "_%s:" name
        sprintf "    mov rax, %d" a
        sprintf "    add rax, %d" b
        sprintf "    mov rdi, rax"
        sprintf "    mov rax, 60 ;sys_exit"
        sprintf "    syscall"
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

let simpleSubtractionProgram (name:string) (a:int) (b:int) =
    [
        sprintf "SECTION .text"
        sprintf ""
        sprintf "_%s:" name
        sprintf "    mov rax, %d" a
        sprintf "    sub rax, %d" b
        sprintf "    mov rdi, rax"
        sprintf "    mov rax, 60 ;sys_exit"
        sprintf "    syscall"
    ]
    |> String.concat Environment.NewLine

[<Property>]
let ``Should map subtraction expression`` (FunctionName name) (a:int) (b:int) =
    {
        EntryPoint = {
            Signature = {Name=name; Parameters=[]; ReturnType="INT"}
            Body = FunctionCall {Function="-"; Inputs=[a;b]} 
        }
        Functions = dict []
    }
    |> buildAssembly
    |> fun result -> result .=. simpleSubtractionProgram name a b