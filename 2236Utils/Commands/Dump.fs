module KPX.AD2236.Commands.Dump

open System
open System.IO

open Newtonsoft.Json

open KPX.AD2236.Model
open KPX.AD2236.Model.UABEA


let dump srcDir projFile =
    let books = ResizeArray<GameBook>()

    for file in Directory.EnumerateFiles(srcDir, "*.json") do
        let book = GameBook.Parse(File.ReadAllText(file))
        books.Add(book)

    let dump =
        [| let inline isOnlyMark str =
               str |> String.forall (fun c -> Char.IsPunctuation(c) && Char.IsWhiteSpace(c))

           for book in books do
               for sheet in book.ImportGridList.Array do
                   let hdr = sheet.Rows.Array.[sheet.HeaderRow].Strings.Array
                   let jpnRow = hdr |> Array.findIndex ((=) "Text")
                   let engRow = hdr |> Array.findIndex ((=) "English")
                   let rowLimit = engRow + 1

                   for row in sheet.Rows.Array do
                       if row.RowIndex <> 0 && row.Strings.Array.Length >= rowLimit then
                           let id = ExportItem.GenerateId(book, sheet, row)
                           let jpn = row.Strings.Array.[jpnRow]
                           let eng = row.Strings.Array.[engRow]

                           if not (isOnlyMark jpn && isOnlyMark eng) then
                               { Id = id
                                 Jpn = jpn
                                 Eng = eng
                                 JpnChs = String.Empty
                                 EngChs = String.Empty
                                 GPTChs = String.Empty
                                 Final = String.Empty } |]

    File.WriteAllText(projFile, JsonConvert.SerializeObject(dump, Formatting.Indented))
