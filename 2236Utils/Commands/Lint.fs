module KPX.AD2236.Commands.Lint

open System
open System.IO
open Newtonsoft.Json.Linq

open KPX.AD2236.Model


let private checkPunctuation = "「」『』—\n".ToCharArray()

type private LintRule =
    { Name: string
      Func: ExportItem -> bool }

let private lintRules =
    [ //{ Name = "半角标点"
      //  Func = (fun item -> item.Final |> String.exists (fun c -> Char.IsAscii c && Char.IsPunctuation c)) }
      { Name = "换行错误"
        Func = 
          (fun item ->
            let ja = item.Jpn.StartsWith('\n')
            let en = item.Final.StartsWith('\n')
            ja <> en) }
      //{ Name = "标点不匹配"
      //  Func =
      //    (fun item ->
      //        checkPunctuation
      //        |> Array.exists (fun pun ->
      //           let ja = item.Jpn.Contains(pun)
      //           let en = item.Final.Contains(pun)
      //           ja <> en)) }
      { Name = "错误标点"
        Func = (fun item -> item.Final.Contains("。」")) } ]

let lint file =
    let maxError = 100
    let mutable currentError = 0

    let json = File.ReadAllText(file)

    for item in JArray.Parse(json).ToObject<ExportItem[]>() do
        for rule in lintRules do
            if rule.Func(item) then
                currentError <- currentError + 1

                Console.WriteLine()
                Console.WriteLine($"ID:{item.Id}")
                Console.WriteLine($"Er:{rule.Name}")
                Console.WriteLine($"Ja:{item.Jpn}")
                Console.WriteLine($"En:{item.Eng}")
                Console.WriteLine($"Tr:{item.Final}")

                if currentError >= maxError then
                    failwith "错误过多，已停止"
