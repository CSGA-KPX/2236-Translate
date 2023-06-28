module KPX.AD2236.Commands.Merge

open System
open System.Collections.Generic
open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open KPX.AD2236.Model
open KPX.AD2236.Model.UABEA


// 尽管GameBook及相关类型都是不可变的，但是Strings里面数组的内容是可以改的
// 改完以后全部重新生成TextLength

type SheetInfo =
    { Sheet: GridList
      JpnIndex: int
      EngIndex: int }

    static member Create(sheet: GridList) =
        let hdr = sheet.Rows.Array.[sheet.HeaderRow].Strings.Array
        let jpnRow = hdr |> Array.findIndex ((=) "Text")
        let engRow = hdr |> Array.findIndex ((=) "English")

        { Sheet = sheet
          JpnIndex = jpnRow
          EngIndex = engRow }


let merge inputDir projFile outputDir =
    let sheetCache = Dictionary<string, SheetInfo>()
    let books = ResizeArray<string * GameBook>()

    for file in Directory.EnumerateFiles(inputDir, "*.json") do
        let jObj = JObject.Parse(File.ReadAllText(file))
        let myObj = jObj.ToObject<GameBook>()
        let outputSrc = JsonConvert.SerializeObject(myObj, Formatting.None)
        let inputSrc = jObj.ToString(Formatting.None)

        if not <| String.Equals(inputSrc, outputSrc) then
            failwithf $"Parser failed : {file}"

        for list in myObj.ImportGridList.Array do
            if list.TextLength <> list.UpdateTextLength().TextLength then
                failwithf $"TextLength failed : {file}"

        for sheet in myObj.ImportGridList.Array do
            let key = $"{myObj.Name}|{sheet.Name}"
            sheetCache.Add(key, SheetInfo.Create(sheet))

        let fileName = Path.GetFileName(file)
        books.Add(fileName, myObj)

    // 清空文件
    Directory.GetFiles(outputDir) |> Array.iter File.Delete

    let translated = JArray.Parse(File.ReadAllText(projFile)).ToObject<ExportItem[]>()

    for item in translated do
        let sheetKey, rowId =
            let idx = item.Id.LastIndexOf('|')
            item.Id.[0 .. idx - 1], int item.Id.[idx + 1 ..]

        let info = sheetCache.[sheetKey]

        // 原文检查
        let src = info.Sheet.Rows.Array.[rowId].Strings.Array.[info.JpnIndex]

        if src <> item.Jpn then
            failwithf $"原文不匹配：\r\nsrc: {src}\r\ntgt:{item.Jpn}"
        // 改写英语
        info.Sheet.Rows.Array.[rowId].Strings.Array.[info.EngIndex] <- item.Final

    for (filename, book) in books do
        let newBook =
            { book with
                ImportGridList =
                    let items = book.ImportGridList.Array |> Array.map (fun l -> l.UpdateTextLength())
                    { Array = items } }


        let outFile = Path.Combine(outputDir, filename)
        File.WriteAllText(outFile, JsonConvert.SerializeObject(newBook, Formatting.Indented))
