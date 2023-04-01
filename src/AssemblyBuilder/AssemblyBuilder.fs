module SharpSilver.AssemblyBuilder

open System

open SharpSilver.AST

module Assembly =
    let rec updateLast l update =
        match l with
        | [] -> []
        | head::[] -> [update head]
        | head::tail -> head :: updateLast tail update

    type Builder() =
        member _.Yield(_) = []

        [<CustomOperation("SECTION")>]
        member _.Section(l, name) =
            l @ [sprintf "SECTION %s%s" name Environment.NewLine]

        [<CustomOperation("mov")>]
        member _.Mov(l, a, b) =
            l @ [sprintf "    mov %O, %O" a b]

        [<CustomOperation("xor")>]
        member _.Xor(l, a, b) =
            l @ [sprintf "    xor %O, %O" a b]
        
        [<CustomOperation("idiv")>]
        member _.Idiv(l, a) =
            l @ [sprintf "    idiv %O" a]

        [<CustomOperation("syscall")>]
        member _.SysCall(l) =
            l @ [sprintf "    syscall"]

        [<CustomOperation("label")>]
        member _.Label(l, a) =
            l @ [sprintf "_%s:" a]

        [<CustomOperation("op2")>]
        member _.Op2(l, name, a, b) =
            l @ [sprintf "    %s %O, %O" name a b]

        [<CustomOperation("comment")>]
        member _.Comment(l, comment) =
            updateLast l (fun s -> sprintf "%s ;%s" s comment)

        member _.Run(l) = String.concat Environment.NewLine l

let assemblyBuilder = new Assembly.Builder()

let rax = "rax"
let rdi = "rdi"
let rdx = "rdx"

let comp = assemblyBuilder {
    mov "rax" 1
    mov "rdx" 2
    mov "rdi" 3
}

let AddSumCode name operation a b =
    assemblyBuilder {
        SECTION ".text"

        label name
        mov rax a
        op2 operation rax b
        mov rdi rax
        mov rax 60 ;comment "sys_exit"
        syscall
    }

let DivCode name a b =
    assemblyBuilder {
        SECTION ".text"

        label name
        mov rax a
        mov rdi b
        xor rdx rdx
        idiv rdi
        mov rdi rax
        mov rax 60 ;comment "sys_exit"
        syscall
    }

let buildAssembly program =
    match program.EntryPoint.Body with
    | ReturnValue value ->
        assemblyBuilder {
            SECTION ".text"

            label program.EntryPoint.Signature.Name
            mov rdi value
            mov rax 60 ;comment "sys_exit"
            syscall
        }
    | FunctionCall {Function=name; Inputs=inputs} ->
        let a = inputs.[0]
        let b = inputs.[1]
        match name with
        | "+" -> AddSumCode program.EntryPoint.Signature.Name "add" a b
        | "-" -> AddSumCode program.EntryPoint.Signature.Name "sub" a b
        | "/" -> DivCode program.EntryPoint.Signature.Name a b
        | _ -> "ARG"
