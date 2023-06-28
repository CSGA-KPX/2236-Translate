module KPX.AD2236.Commands.GoogleTranslate

open System
open System.Collections.Generic
open System.IO
open System.Text

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open SwaggerProvider

open KPX.AD2236.Model


[<Literal>]
let private schema = "Commands/deep-translator-api.json"
let private endpoint = "http://localhost:8000/"
let private boundery = "b271b1dfc4e44702be1e1f464b544015"
let private batchSize = 4000

type DeepAPI = OpenApiClientProvider<schema>

type SourceLanguage =
    | English
    | Japanese

let translate projFile (lang: SourceLanguage) =
    let langCode, textFunc, todoFunc =
        match lang with
        | English ->
            "en-US", (fun (item: ExportItem) -> item.Eng), (fun (item: ExportItem) -> String.IsNullOrEmpty(item.EngChs))
        | Japanese ->
            "ja", (fun (item: ExportItem) -> item.Jpn), (fun (item: ExportItem) -> String.IsNullOrEmpty(item.JpnChs))

    let data = JArray.Parse(File.ReadAllText(projFile)).ToObject<ExportItem[]>()

    let batches =
        let data = data |> Array.filter todoFunc |> Queue<ExportItem>
        let pool = Queue<ExportItem>()
        let sb = StringBuilder()

        [ while data.Count <> 0 do
              if sb.Length >= batchSize then
                  sb.Length <- sb.Length - boundery.Length - 4
                  yield (pool.ToArray(), sb.ToString())
                  pool.Clear()
                  sb.Clear() |> ignore
              else
                  let item = data.Dequeue()
                  pool.Enqueue(item)
                  sb.AppendLine(textFunc item).AppendLine(boundery) |> ignore

          if pool.Count <> 0 then
              sb.Length <- sb.Length - boundery.Length - 4
              yield (pool.ToArray(), sb.ToString())
              pool.Clear()
              sb.Clear() |> ignore ]

    printfn $"总计%5i{data.Length}条"
    printfn $"批量翻译：共{batches.Length}批"

    let client =
        let httpClient = new Net.Http.HttpClient(BaseAddress = Uri(endpoint))
        DeepAPI.Client(httpClient)

    let mutable continueProcess = true

    Console.CancelKeyPress.Add(fun e ->
        e.Cancel <- true
        continueProcess <- false)

    for (items, string) in batches do
        if continueProcess then
            let ret =
                let post = DeepAPI.GoogleRequest("zh-CN", string, langCode)

                client
                    .GoogleTranslateGooglePost(post)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult()

            if String.IsNullOrWhiteSpace(ret.Error) then
                let chunks = ret.Translation.Split($"\r\n{boundery}\r\n")

                Array.iter2
                    (fun item chunk ->
                        Console.WriteLine()
                        Console.WriteLine($"Id：{item.Id}")
                        Console.WriteLine($"日：{item.Jpn}")
                        Console.WriteLine($"英：{item.Eng}")
                        Console.WriteLine($"译：{chunk}")
                        item.EngChs <- chunk)
                    items
                    chunks

            else
                Console.WriteLine($"API调用失败，终止")
                Console.WriteLine(ret.Error)
                failwithf "API调用失败"
                continueProcess <- false

            System.Threading.Thread.Sleep(1000)

    let json = JsonConvert.SerializeObject(data, Formatting.Indented)
    File.WriteAllText(projFile, json)
    Console.WriteLine("进度已保存")
