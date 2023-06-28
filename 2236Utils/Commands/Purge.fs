module KPX.AD2236.Commands.Purge

open System
open System.IO


let purge () =
    let path =
        let local =
            Environment.SpecialFolder.LocalApplicationData |> Environment.GetFolderPath

        Path.Combine(local, @"..\LocalLow\Unity\Chloro_2236 A_D_ -Universal Edition-")

    Directory.Delete(path, true)
