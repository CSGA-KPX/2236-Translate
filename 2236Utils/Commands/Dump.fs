module KPX.AD2236.Commands.Dump

open System
open System.IO

open Newtonsoft.Json

open KPX.AD2236.Model
open KPX.AD2236.Model.UABEA


let private orderOverride =
    [ "2236_00.book"
      "2236_00R.book"
      "2236_01.book"
      "2236_02.book"
      "2236_03.book"
      "2236_04.book"
      "2236_05.book"
      "2236_06R.book"
      "2236_06.book"
      "2236_07R.book"
      "2236_07.book"
      "2236_08R.book"
      "2236_08.book"
      "2236_09.book"
      "2236_10.book"
      "2236_11.book"
      "2236_12.book"
      "2236_13R.book"
      "2236_13.book"
      "2236_14.book"
      "2236_15.book"
      "2236_16R.book"
      "2236_16.book"
      "2236_16y.book"
      "2236_17R.book"
      "2236_17.book"
      "2236_17y.book"
      "2236_18R.book"
      "2236_18.book"
      "2236_18hR.book"
      "2236_18h.book"
      "2236_18y.book"
      "2236_19R.book"
      "2236_19.book" ]
    |> List.mapi (fun idx str -> str, idx)
    |> readOnlyDict

let dump srcDir projFile =
    let books = ResizeArray<GameBook>()

    let sorted =
        Directory.EnumerateFiles(srcDir, "*.json")
        |> Seq.toArray
        |> Array.sortBy (fun path ->
            let name = Path.GetFileName(path)
            let book = name.[0 .. name.IndexOf('-') - 1]
            orderOverride.[book])

    for file in sorted do
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
