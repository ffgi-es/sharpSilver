module Program

open FsCheck.Xunit
open SharedUtilities

[<assembly: Properties(Arbitrary= [|typeof<ArbitraryModifiers>|])>] do()

let [<EntryPoint>] main _ = 0
