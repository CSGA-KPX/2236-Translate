module KPX.AD2236.Commands.GalTransDump

open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open KPX.AD2236.Model


let dump proj outFile =
    let json = File.ReadAllText(proj)
    let data = JArray.Parse(json).ToObject<ExportItem[]>()

    let data2 = data |> Array.map (fun item -> { GalTrans.Message = item.Jpn })

    File.WriteAllText(outFile, JsonConvert.SerializeObject(data2, Formatting.Indented))
