module KPX.AD2236.Commands.Validate

open System.IO
open Newtonsoft.Json.Linq

open KPX.AD2236.Model



let validate file =
    let json = File.ReadAllText(file)
    JArray.Parse(json).ToObject<ExportItem[]>() |> ignore