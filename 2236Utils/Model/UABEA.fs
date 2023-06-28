namespace KPX.AD2236.Model.UABEA

open System

open Newtonsoft.Json
open Newtonsoft.Json.Linq


type ArrayWarper<'T> = { Array: 'T[] }

type FileObject =
    { [<JsonProperty("m_FileID")>]
      FileId: int64
      [<JsonProperty("m_PathID")>]
      PathId: int64 }

type Row =
    { [<JsonProperty("rowIndex")>]
      RowIndex: int
      [<JsonProperty("strings")>]
      Strings: ArrayWarper<string>
      [<JsonProperty("isEmpty")>]
      isEmpty: int
      [<JsonProperty("isCommentOut")>]
      isCommentOut: int }

type GridList =
    { [<JsonProperty("rows")>]
      Rows: ArrayWarper<Row>
      [<JsonProperty("name")>]
      Name: string
      [<JsonProperty("type")>]
      // 0=csv 1=tsv
      Type: int
      [<JsonProperty("textLength")>]
      TextLength: int
      [<JsonProperty("headerRow")>]
      HeaderRow: int
      // 没有使用
      [<JsonProperty("entityIndexTbl")>]
      EntityIndexTbl: ArrayWarper<int>
      [<JsonProperty("entityDataList")>]
      // 没有使用
      EntityDataList: ArrayWarper<int> }

    member x.UpdateTextLength() =
        let mutable len = 0

        for row in x.Rows.Array do
            for str in row.Strings.Array do
                len <- len + str.Length

        { x with TextLength = len }

type GameBook =
    { [<JsonProperty("m_GameObject")>]
      GameObject: FileObject
      [<JsonProperty("m_Enabled")>]
      Enabled: int
      [<JsonProperty("m_Script")>]
      Script: FileObject
      [<JsonProperty("m_Name")>]
      Name: string
      [<JsonProperty("importVersion")>]
      ImportVersion: int
      [<JsonProperty("importGridList")>]
      ImportGridList: ArrayWarper<GridList> }

    static member Parse(json: string) =
        let jsonObj = JObject.Parse(json)
        let book = jsonObj.ToObject<GameBook>()

        let json = jsonObj.ToString(Formatting.None)
        let bookJson = JsonConvert.SerializeObject(book, Formatting.None)

        if not <| String.Equals(json, bookJson) then
            failwithf $"Parser failed"


        for list in book.ImportGridList.Array do
            if list.TextLength <> list.UpdateTextLength().TextLength then
                failwithf $"TextLength failed"

        book
