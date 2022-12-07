module SharpSilver.Parser

open System.Text.RegularExpressions

open AST

let parseFromString source =
    let regex = new Regex(@"(-?\d+)")
    let m = regex.Match(source)
    {
        Signature={Name="main"; Parameters=[]; ReturnType="INT"}
        Body = ReturnValue (int m.Groups[1].Value)
    }