open System
open System.IO

open KPX.AD2236.Commands


[<Literal>]
let helpText =
    """2236utils command args

        projFile   EnvironmentVar

            dump   jsonPath 
           merge   jsonPath savePath
        validate   
   googleTransEn   
   googleTransJa   
    GalTransDump   outFile
   GalTransMerge   transFile 
           Purge   <no args>
         Context   (id or id list file)
            Lint   
        Progress   id
          Search   lang str
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
    let projFile = Environment.GetEnvironmentVariable("projFile")

    if args.Length < 1 then
        Console.WriteLine(helpText)
    else
        args.[0] <- args.[0].ToLowerInvariant()

        try
            match args with
            | [| "dump"; srcPath |] -> Dump.dump srcPath projFile
            | [| "merge"; jsonPath; savePath |] ->
                ensureFolderExists jsonPath
                emptyFolder savePath "*.json"
                ensureFileExists projFile
                Merge.merge jsonPath projFile savePath
            | [| "validate" |] ->

                ensureFileExists projFile
                Validate.validate projFile
            | [| "lint" |] ->
                ensureFileExists projFile
                Lint.lint projFile
            | [| "googletransen" |] ->
                ensureFileExists projFile
                GoogleTranslate.translate projFile GoogleTranslate.English
            | [| "googletransja" |] ->
                ensureFileExists projFile
                GoogleTranslate.translate projFile GoogleTranslate.Japanese
            | [| "galtransdump"; transFile |] ->
                ensureFileExists projFile
                GalTransDump.dump projFile transFile
            | [| "galtransmerge"; transFile |] ->
                ensureFileExists transFile
                ensureFileExists projFile
                GalTransMerge.merge transFile projFile
            | [| "purge" |] -> Purge.purge ()
            | [| "context"; str |] ->
                ensureFileExists projFile
                Context.get projFile str
            | [| "progress"; id |] ->
                ensureFileExists projFile
                Progress.check projFile id
            | [| "search"; lang; str |] ->
                ensureFileExists projFile
                Search.search projFile lang str

            | _ ->
                printfn $"未知指令:%A{args}"
                Console.WriteLine(helpText)
        with e ->
            printfn $"{e.ToString()}"

    0
