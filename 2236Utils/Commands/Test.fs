module KPX.AD2236.Commands.Test

open System.IO
open System.Text
open System.Text.RegularExpressions

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open KPX.AD2236.Model


let test projFile =

    let ellipsis = Regex("\.{3,}")

    let data =
        let json = File.ReadAllText(projFile)

        JArray.Parse(json).ToObject<ExportItem[]>()
        |> Array.map (fun x ->
            let final = x.Final.Replace("。」", "」")
            let final = ellipsis.Replace(final, "……")
            { x with Final = final })

    File.WriteAllText(projFile, JsonConvert.SerializeObject(data, Formatting.Indented))
