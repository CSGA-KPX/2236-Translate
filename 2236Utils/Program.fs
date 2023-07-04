open System
open System.IO

open KPX.AD2236.Commands


[<Literal>]
let helpText =
    """2236utils command args

            dump   jsonPath projFile
           merge   jsonPath projFile savePath
        validate   projFile
   googleTransEn   projFile
   googleTransJa   projFile
    GalTransDump   projFile outFile
   GalTransMerge   transFile projFile
           Purge   <no args>
           Blame   projFile (id or id list file)
            Lint   projFile
"""

let inline ensureFileExists file =
    if not <| File.Exists(file) then
        invalidOp $"文件{file}不存在"

let inline ensureFolderExists folder =
    if not <| Directory.Exists(folder) then
        invalidOp $"目录{folder}不存在"

let inline emptyFolder path pattern =
    ensureFolderExists (path)

    for file in Directory.EnumerateFiles(path, pattern) do
        File.Delete(file)

[<EntryPoint>]
let main args =
    if args.Length < 2 then
        Console.WriteLine(helpText)
    else
        args.[0] <- args.[0].ToLowerInvariant()

        try
            match args with
            | [| "dump"; srcPath; projFile |] ->
                ensureFolderExists srcPath
                ensureFileExists projFile
                Dump.dump srcPath projFile
            | [| "merge"; jsonPath; projFile; savePath |] ->
                ensureFolderExists jsonPath
                ensureFileExists projFile
                emptyFolder savePath "*.json"
                Merge.merge jsonPath projFile savePath
            | [| "validate"; projFile |] ->
                ensureFileExists projFile
                Validate.validate projFile
            | [| "lint"; projFile |] ->
                ensureFileExists projFile
                Lint.lint projFile
            | [| "googletransen"; projFile |] ->
                ensureFileExists projFile
                GoogleTranslate.translate projFile GoogleTranslate.English
            | [| "googletransja"; projFile |] ->
                ensureFileExists projFile
                GoogleTranslate.translate projFile GoogleTranslate.Japanese
            | [| "galtransdump"; projFile; transFile |] ->
                ensureFileExists projFile
                GalTransDump.dump projFile transFile
            | [| "galtransmerge"; transFile; projFile |] ->
                ensureFileExists transFile
                ensureFileExists projFile
                GalTransMerge.merge transFile projFile
            | [| "purge" |] -> Purge.purge ()
            | [| "blame"; projFile; str |] ->
                ensureFileExists projFile
                Blame.blame projFile str
            | _ -> printfn $"未知指令:%A{args}"
        with e ->
            printfn $"{e.ToString()}"

    0
