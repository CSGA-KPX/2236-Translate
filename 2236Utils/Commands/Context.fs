module KPX.AD2236.Commands.Context

open System
open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open KPX.AD2236.Model


let private toId str =
    [ if File.Exists(str) then yield! File.ReadAllLines(str)
      elif str.Contains("") then yield str
      else invalidArg "str" "输入不是ID或者ID列表文件" ]

// 生成-5 .. +5个条目
let get projFile str =
    let data = 
        let json = File.ReadAllText(projFile)
        JArray.Parse(json).ToObject<ExportItem[]>()

    let idDict = 
        data
        |> Seq.mapi (fun idx item -> (item.Id, idx))
        |> readOnlyDict

    let ids = toId str
    for id in ids do
        Console.WriteLine($">>{id}")
        if idDict.ContainsKey(id) then
            let idx = idDict.[id]
            for idx in [idx - 5 .. idx + 5] do
                let item = data.[idx]
                Console.WriteLine("")
                if item.Id = id then
                    Console.WriteLine($"*{item.Id}*")
                else
                    Console.WriteLine(item.Id)
                Console.WriteLine($"Ja:{item.Jpn}")
                Console.WriteLine($"En:{item.Eng}")
                Console.WriteLine($"Tr:{item.Final}")
        else
            Console.WriteLine($"找不到{id}")
        Console.WriteLine("")
        Console.WriteLine($"<<{id}")
