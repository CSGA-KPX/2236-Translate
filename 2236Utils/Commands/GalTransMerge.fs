module KPX.AD2236.Commands.GalTransMerge

open System
open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open KPX.AD2236.Model
open KPX.AD2236.Model.GalTrans


let private replaceTable = [ "“", "「"; "”", "」"; "‘", "『"; "’", "』" ]

let merge transFile projFile =
    let dump = JArray.Parse(File.ReadAllText(projFile)).ToObject<ExportItem[]>()

    let trans =
        JArray
            .Parse(File.ReadAllText(transFile))
            .ToObject<GalTrans.IntermediateMessage[]>()

    dump
    |> Array.iteri2
        (fun index trans dump ->
            let eq = String.Equals(trans.PreJP, dump.Jpn)

            if not eq then
                printfn "jpn:%s" dump.Jpn
                printfn "tra:%s" trans.PreJP

                Console.ReadLine() |> ignore
            else
                let sb = Text.StringBuilder(trans.PreZH)

                for (f, t) in replaceTable do
                    sb.Replace(f, t) |> ignore

                let rep = sb.ToString()
                dump.GPTChs <- rep
                dump.Final <- rep)
        trans

    File.WriteAllText(projFile, JsonConvert.SerializeObject(dump, Formatting.Indented))
