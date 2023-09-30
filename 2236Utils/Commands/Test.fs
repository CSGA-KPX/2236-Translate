module KPX.AD2236.Commands.Test

open System.IO
open System.Text
open System.Text.RegularExpressions

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open KPX.AD2236.Model


let bookMapping = 
    [ 
        "2236_00.book|Assets/2236/2236_00.xlsx", "2236_00R.book|Assets/2236/2236_00R.xlsx"
        "2236_00.book|Assets/2236/2236_06.xlsx", "2236_00R.book|Assets/2236/2236_06R.xlsx"
        "2236_00.book|Assets/2236/2236_07.xlsx", "2236_00R.book|Assets/2236/2236_07R.xlsx"
        "2236_00.book|Assets/2236/2236_08.xlsx", "2236_00R.book|Assets/2236/2236_08R.xlsx"
        "2236_00.book|Assets/2236/2236_13.xlsx", "2236_00R.book|Assets/2236/2236_13R.xlsx"
        "2236_00.book|Assets/2236/2236_16.xlsx", "2236_00R.book|Assets/2236/2236_16R.xlsx"
        "2236_00.book|Assets/2236/2236_17.xlsx", "2236_00R.book|Assets/2236/2236_17R.xlsx"
        "2236_00.book|Assets/2236/2236_18.xlsx", "2236_00R.book|Assets/2236/2236_18R.xlsx"
        "2236_00.book|Assets/2236/2236_18h.xlsx", "2236_00R.book|Assets/2236/2236_18hR.xlsx"
        "2236_00.book|Assets/2236/2236_19.xlsx", "2236_00R.book|Assets/2236/2236_19R.xlsx"
    ]

let test (fileFrom : string) (fromTo : string) =

    let src = 
        let json = File.ReadAllText(fileFrom)
        JArray.Parse(json).ToObject<ExportItem[]>()

    let dst = 
        let json = File.ReadAllText(fromTo)
        JArray.Parse(json).ToObject<ExportItem[]>()

    let ellipsis = Regex("\.{3,}")

    let data =
        let json = File.ReadAllText(fileFrom)

        JArray.Parse(json).ToObject<ExportItem[]>()
        |> Array.map (fun x ->
            let final = x.Final.Replace("。」", "」")
            let final = ellipsis.Replace(final, "……")
            { x with Final = final })

    File.WriteAllText(fileFrom, JsonConvert.SerializeObject(data, Formatting.Indented))
