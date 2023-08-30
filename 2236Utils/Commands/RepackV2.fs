module RepackV2

open System
open System.IO
open System.Collections.Generic

open AssetsTools.NET
open AssetsTools.NET.Extra

open UABEAvalonia

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open KPX.AD2236.Model
open KPX.AD2236.Model.UABEA
open KPX.AD2236.Commands.Merge


let repack  (originAsset: string)  projFile targetAsset =
    let am = AssetsManager()
    let bws = BundleWorkspace()

    let tpkFile =
        let exePath =
            Reflection.Assembly.GetExecutingAssembly().Location |> Path.GetDirectoryName

        Path.Combine(exePath, "classdata.tpk")

    am.LoadClassPackage(tpkFile) |> ignore

    let bun = am.LoadBundleFile(originAsset, true)
    bws.Reset(bun)

    let item = bws.Files.[0]
    let assetMemPath = Path.Combine(bun.path, item.Name)
    item.Stream.Position <- 0
    let fileInst = am.LoadAssetsFile(item.Stream, assetMemPath, true)
    fileInst.parentBundle <- bun

    am.LoadClassDatabaseFromPackage(fileInst.file.Metadata.UnityVersion) |> ignore

    let aws = AssetWorkspace(am, true)

    aws.LoadAssetsFile(fileInst, true)

    //let ucont = UnityContainer()

    //for file in aws.LoadedFiles do
    //    let succ, actualFile, ucontBaseField =
    //        UnityContainer.TryGetBundleContainerBaseField(aws, file)

    //    if succ then
    //        ucont.FromAssetBundle(am, actualFile, ucontBaseField)
    //    else
    //        let succ, actualFile, ucontBaseField =
    //            UnityContainer.TryGetRsrcManContainerBaseField(aws, file)

    //        if succ then
    //            ucont.FromResourceManager(am, actualFile, ucontBaseField)

    //    for asset in aws.LoadedAssets do
    //        let pptr = AssetPPtr(asset.Key.fileName, 0, asset.Key.pathID)
    //        let path = ucont.GetContainerPath(pptr)

    //        if not <| isNull path then
    //            asset.Value.Container <- path


    //aws.GenerateAssetsFileLookup()



    let sheetCache = Dictionary<string, SheetInfo>()
    let books = ResizeArray<string * GameBook>()
    let bookCont = Dictionary<string, AssetContainer>()

    for cont in aws.LoadedAssets.Values do
        let (name, typeName) = Extensions.GetUABENameFast(aws, cont, true)
        printfn $"Read : name = {name}, typeName = {typeName}"

        if name.EndsWith("book") then
            printfn $"Reading {name}"

            let bf = am.GetBaseField(cont.FileInstance, cont.AssetPPtr.PathId)
            use ms = new MemoryStream()
            use sw = new StreamWriter(ms, AutoFlush = true)
            let tmpexp = AssetImportExport()
            tmpexp.DumpJsonAsset(sw, bf)

            let myObj =
                JObject.Parse(Text.Encoding.UTF8.GetString(ms.ToArray())).ToObject<GameBook>()

            for list in myObj.ImportGridList.Array do
                if list.TextLength <> list.UpdateTextLength().TextLength then
                    failwithf $"TextLength failed : {name}"

            for sheet in myObj.ImportGridList.Array do
                let key = $"{myObj.Name}|{sheet.Name}"
                sheetCache.Add(key, SheetInfo.Create(sheet))

            books.Add(name, myObj)
            bookCont.Add(name, cont)

    printfn $"Read end: len(SheetCache)={sheetCache.Count} len(books)={books.Count}"

    printfn $"Reading projFile {projFile}"

    let translated = JArray.Parse(File.ReadAllText(projFile)).ToObject<ExportItem[]>()

    for item in translated do
        let sheetKey, rowId =
            let idx = item.Id.LastIndexOf('|')
            item.Id.[0 .. idx - 1], int item.Id.[idx + 1 ..]

        let info = sheetCache.[sheetKey]

        // 原文检查
        let src = info.Sheet.Rows.Array.[rowId].Strings.Array.[info.JpnIndex]

        if src <> item.Jpn then
            failwithf $"原文不匹配：\r\nsrc: {src}\r\ntgt:{item.Jpn}"
        // 改写英语
        info.Sheet.Rows.Array.[rowId].Strings.Array.[info.EngIndex] <- item.Final


    for (name, book) in books do
        printfn $"Recalculating text length for {name}"
        let newBook =
            { book with
                ImportGridList =
                    let items = book.ImportGridList.Array |> Array.map (fun l -> l.UpdateTextLength())
                    { Array = items } }
        printfn $"Updating asset for {name}"
        use ms =
            let json = JsonConvert.SerializeObject(newBook)
            let bytes = Text.Encoding.UTF8.GetBytes(json)
            new MemoryStream(bytes)

        use sr = new StreamReader(ms)
        let importer = AssetImportExport()
        let tempField = aws.GetTemplateField(bookCont.[name])
        let bytes, error = importer.ImportJsonAsset(tempField,sr)
        if isNull bytes then
            failwithf $"Processing {name} failed: %A{error}"

        let replacer = AssetImportExport.CreateAssetReplacer(bookCont.[name], bytes)
        aws.AddReplacer(bookCont.[name].FileInstance, replacer)//, ms)

    printfn $"Saving assets"
    
    let fileToReplacer = Dictionary<AssetsFileInstance, List<AssetsReplacer>>()

    aws.GenerateAssetsFileLookup()

    for key in aws.LoadedFileLookup.Keys do
        printfn $"Loaded {key}"

    for newAsset in aws.NewAssets do
        printfn $"{newAsset.Key.pathID} is new."
        let assetId = newAsset.Key
        let replacer = newAsset.Value
        let fileName = assetId.fileName
        printfn $"searching {fileName}"
        let succ, file = aws.LoadedFileLookup.TryGetValue(fileName.ToLower())
        if succ then
            if not <| fileToReplacer.ContainsKey(file) then
                fileToReplacer.[file] <- ResizeArray<AssetsReplacer>()

            fileToReplacer.[file].Add(replacer)

    printfn $"len(fileToReplacer)={fileToReplacer.Count}. {aws.GetChangedFiles().Count} files changed."

    let changedAssetsDatas = ResizeArray<AssetsFileInstance * byte[]>()

    for changed in aws.GetChangedFiles() do
        let replacers = 
            if fileToReplacer.ContainsKey(changed) then
                fileToReplacer.[changed]
            else
                ResizeArray<_>()

        use ms = new MemoryStream()
        use w = new AssetsFileWriter(ms)
        printfn $"Applying changed file {changed} with %A{replacers}"
        changed.file.Write(w, 0, replacers)
        changedAssetsDatas.Add(changed, ms.ToArray())
        //ChangedAssetsDatas.Add(new Tuple<AssetsFileInstance, byte[]>(file, ms.ToArray()));

    am.UnloadAllAssetsFiles(true) |> ignore
    printfn $"Assets saved"

    printfn $"Saving bundle"
        
    for (fi, data) in changedAssetsDatas do
        let assetName = Path.GetFileName(fi.path)
        printfn $"bws.AddOrReplaceFile {fi.path}"
        bws.AddOrReplaceFile(new MemoryStream(data), assetName, true)
        am.UnloadAssetsFile(fi.path) |> ignore

    let replacers = bws.GetReplacers()
    use fs = File.Open(targetAsset, FileMode.Create)
    use w = new AssetsFileWriter(fs)
    bun.file.Write(w, replacers)


    printfn $"Bundle saved."

    printfn $"Purging game cache."
    try
        KPX.AD2236.Commands.Purge.purge()
    with
    | _ -> ()
    printfn $"Purged."

    printfn $"All operation completed."
    ()
