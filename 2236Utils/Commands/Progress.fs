module KPX.AD2236.Commands.Progress

open System
open System.IO
open Newtonsoft.Json.Linq

open KPX.AD2236.Model


let check file (str: string) =
    let data =
        let json = File.ReadAllText(file)
        JArray.Parse(json).ToObject<ExportItem[]>()

    let id = data |> Array.findIndex (fun item -> item.Id.Contains(str))

    let progress = (float id) / (float data.Length)

    printfn $"当前进度{data.[id].Id}"
    printfn $"位于{id}/{data.Length} : 总计 %.1f{progress * 100.0}%%"
