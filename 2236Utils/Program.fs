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
    let projFile =
        let path = Environment.GetEnvironmentVariable("projFile")
        ensureFileExists path
        path

    if args.Length < 1 then
        Console.WriteLine(helpText)
    else
        args.[0] <- args.[0].ToLowerInvariant()

        try
            match args with
            | [| "dump"; srcPath |] ->
                ensureFolderExists srcPath
                Dump.dump srcPath projFile
            | [| "merge"; jsonPath; savePath |] ->
                ensureFolderExists jsonPath
                emptyFolder savePath "*.json"
                Merge.merge jsonPath projFile savePath
            | [| "validate" |] -> Validate.validate projFile
            | [| "lint" |] -> Lint.lint projFile
            | [| "googletransen" |] -> GoogleTranslate.translate projFile GoogleTranslate.English
            | [| "googletransja" |] -> GoogleTranslate.translate projFile GoogleTranslate.Japanese
            | [| "galtransdump"; transFile |] -> GalTransDump.dump projFile transFile
            | [| "galtransmerge"; transFile |] ->
                ensureFileExists transFile
                GalTransMerge.merge transFile projFile
            | [| "purge" |] -> Purge.purge ()
            | [| "ontext"; str |] -> Context.get projFile str
            | [| "progress"; id |] -> Progress.check projFile id
            | [| "search"; lang; str |] -> Search.search projFile lang str

            | _ -> 
                printfn $"未知指令:%A{args}"
                Console.WriteLine(helpText)
        with e ->
            printfn $"{e.ToString()}"

    0
