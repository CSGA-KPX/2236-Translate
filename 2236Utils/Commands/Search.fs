module KPX.AD2236.Commands.Search

open System
open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open KPX.AD2236.Model


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

    data
    |> Seq.filter (fun item -> (lang item).Contains(str))
    |> Seq.iter (fun item ->
        Console.WriteLine("")
        Console.WriteLine(item.Id)
        Console.WriteLine($"Ja:{item.Jpn}")
        Console.WriteLine($"En:{item.Eng}")
        Console.WriteLine($"Tr:{item.Final}"))
