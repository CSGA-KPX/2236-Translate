module KPX.AD2236.Commands.DiffMerge

open System
open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open KPX.AD2236.Model

type Message =
    { [<JsonProperty("message")>]
      Message: string
      [<JsonProperty("remove")>]
      Remove: string
      [<JsonProperty("keep")>]
      Keep: string
      [<JsonProperty("replace")>]
      Replace: string }

    member x.ToRemove = String.IsNullOrWhiteSpace(x.Remove) |> not
    member x.ToKeep = String.IsNullOrWhiteSpace(x.Keep) |> not
    member x.IsPadding = x.Message = "PADDING"
    member x.ToReplace = String.IsNullOrWhiteSpace(x.Replace) |> not

let diffMerge diffFile projFile oriFile targetFile outFile =
    let src =
        let ori = JArray.Parse(File.ReadAllText(oriFile)).ToObject<ExportItem[]>()
        let src = JArray.Parse(File.ReadAllText(projFile)).ToObject<ExportItem[]>()
        let dict = Collections.Generic.Dictionary<string, ExportItem>(src.Length)

        for item in src do
            dict.Add(item.Id, item)
        // restore order
        ori |> Array.map (fun item -> dict.[item.Id])

    let dst = JArray.Parse(File.ReadAllText(targetFile)).ToObject<ExportItem[]>()
    let diff = JArray.Parse(File.ReadAllText(diffFile)).ToObject<Message[]>()

    let mutable srcIdx = 0
    let mutable dstIdx = 0

    for item in diff do
        let si = src.[srcIdx]
        let di = dst.[dstIdx]


        printfn $"{si.Id} -> {di.Id} \r\n {item}"

        if (si.Jpn = di.Jpn) || (item.ToKeep) then
            // 要么一样，要么细微差别(keep=true)
            dst.[dstIdx] <-
                { dst.[dstIdx] with
                    JpnChs = si.JpnChs
                    EngChs = si.EngChs
                    GPTChs = si.GPTChs
                    Final = si.Final }

            srcIdx <- srcIdx + 1
            dstIdx <- dstIdx + 1
        elif item.ToRemove then
            // 原文件中多出来的
            srcIdx <- srcIdx + 1
        else if item.IsPadding then
            // 新加的
            dst.[dstIdx] <-
                { dst.[dstIdx] with
                    Final = dst.[dstIdx].Jpn }
            dstIdx <- dstIdx + 1
        elif item.ToReplace then
            // 需要替换的
            dst.[dstIdx] <-
                { dst.[dstIdx] with
                    Final = dst.[dstIdx].Jpn }
            srcIdx <- srcIdx + 1
            dstIdx <- dstIdx + 1
        else
            failwithf $"{si.Jpn} <--> {di.Jpn}"


    File.WriteAllText(outFile, JsonConvert.SerializeObject(dst, Formatting.Indented))
