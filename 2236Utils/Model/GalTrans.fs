namespace KPX.AD2236.Model.GalTrans

open Newtonsoft.Json


type Message =
    { [<JsonProperty("message")>]
      Message: string }

type IntermediateMessage =
    { 
      [<JsonProperty("index")>]
      Index: int
      [<JsonProperty("name")>]
      Name: string
      [<JsonProperty("pre_jp")>]
      PreJP: string
      [<JsonProperty("post_jp")>]
      PostJP: string
      [<JsonProperty("pre_zh")>]
      PreZH: string
      [<JsonProperty("proofread_zh")>]
      ProofreadZh: string
      [<JsonProperty("trans_by")>]
      TransBy: string
      [<JsonProperty("proofread_by")>]
      ProofreadBy: string
      [<JsonProperty("post_zh_preview")>]
      PostZhPreview: string }