module SharpSilver.Compile

open System
open System.IO

open SharpSilver.Parser
open SharpSilver.AssemblyBuilder

let writeCode (source:string) =
    File.AppendAllText("./test.asm", source)

let runCommand command =
    let proc = System.Diagnostics.Process.Start("/usr/bin/env", ["-S"; "bash" ; "-c"; command])
    proc.WaitForExit()
    proc.ExitCode

let compileAssembly () = runCommand "nasm -felf64 -o test.o test.asm"

let linkExecutable () = runCommand "ld -e _main -o test test.o"    

[<EntryPoint>]
let main args =
    parseFromFile args.[0]
    |> function
        | Success program -> buildAssembly program
        | Failure error ->
            eprintfn "%s" error
            exit 1
    |> writeCode
    |> compileAssembly
    |> ignore
    |> linkExecutable
