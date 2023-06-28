namespace KPX.AD2236.Model

open Newtonsoft.Json

open KPX.AD2236.Model.UABEA


type ExportItem =
    { Id: string
      Jpn: string
      Eng: string
      [<JsonProperty("Jpn-Chs")>]
      mutable JpnChs: string
      [<JsonProperty("Eng-Chs")>]
      mutable EngChs: string
      mutable GPTChs : string
      mutable Final: string}

    static member GenerateId(book: GameBook, sheet: GridList, row: Row) =
        $"{book.Name}|{sheet.Name}|{row.RowIndex}"