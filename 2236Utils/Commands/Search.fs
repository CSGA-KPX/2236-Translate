module KPX.AD2236.Commands.Search

open System
open System.Collections.Generic
open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open KPX.AD2236.Model

#nowarn "57"


// 生成-5 .. +5个条目
let search projFile (lang: string) (str: string) =

    let lang =
        match lang.ToLowerInvariant() with
        | "ja"
        | "jpn" -> fun (item: ExportItem) -> item.Jpn
        | "en"
        | "eng" -> fun (item: ExportItem) -> item.Eng
        | "zh"
        | "chs"
        | "final" -> fun (item: ExportItem) -> item.Final
        | _ -> failwith "语言不存在"

    let data =
        let json = File.ReadAllText(projFile)
        JArray.Parse(json).ToObject<ExportItem[]>()

    let linesDict =
        let lines = File.ReadAllLines(projFile)
        let linesPerItem = typeof<ExportItem>.GetProperties().Length + 2
        let dict = Dictionary<string, int>(lines.Length / linesPerItem)

        for lineNo = 0 to lines.Length - 1 do
            let str = lines.[lineNo]

            if str.Contains("\"Id\": ") then
                let id = str.Split(": ").[1].Trim([| '"'; ',' |])
                dict.Add(id, lineNo)

        dict

    data
    |> Array.Parallel.filter (fun item -> (lang item).Contains(str))
    |> Array.iter (fun item ->
        Console.WriteLine("")
        Console.WriteLine(item.Id)
        Console.WriteLine($"Ln:{linesDict.[item.Id]}")
        Console.WriteLine($"Ja:{item.Jpn}")
        Console.WriteLine($"En:{item.Eng}")
        Console.WriteLine($"Tr:{item.Final}"))
