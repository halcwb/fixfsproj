namespace Informedica.fixfsproj

module Fix =

    open System
    open System.IO
    open System.Xml
    open System.Text.RegularExpressions

    [<Literal>]
    let currDir = @"c:\Users\hal\Dropbox\Development\"
    [<Literal>]
    let oldVersion = @"	  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">"
    [<Literal>]
    let newVersion = @"	<TargetFSharpCoreVersion>4.3.0.0</TargetFSharpCoreVersion>
      </PropertyGroup>
      <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">"
    [<Literal>]
    let oldImport = @"<Import Project=""$(MSBuildExtensionsPath32)\..\Microsoft SDKs\F#\3.0\Framework\v4.0\Microsoft.FSharp.Targets"" Condition="" Exists('$(MSBuildExtensionsPath32)\..\Microsoft SDKs\F#\3.0\Framework\v4.0\Microsoft.FSharp.Targets')"" />"
    [<Literal>]
    let newImport = @"<Choose>
	      <When Condition=""'$(VisualStudioVersion)' == '11.0'"">
		    <PropertyGroup>
		      <FSharpTargetsPath>$(MSBuildExtensionsPath32)\..\Microsoft SDKs\F#\3.0\Framework\v4.0\Microsoft.FSharp.Targets</FSharpTargetsPath>
		    </PropertyGroup>
	      </When>
	      <Otherwise>
		    <PropertyGroup>
		      <FSharpTargetsPath>$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\FSharp\Microsoft.FSharp.Targets</FSharpTargetsPath>
		    </PropertyGroup>
	      </Otherwise>
	    </Choose>
        <Import Project=""$(FSharpTargetsPath)"" Condition=""Exists('$(FSharpTargetsPath)')"" />"
    [<Literal>]
    let oldInclude = @"    <Reference Include=""FSharp.Core, Version=4.3.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"">
          <Private>True</Private>"
    [<Literal>]
    let newInclude = @"	<Reference Include='FSharp.Core, Version=$(TargetFSharpCoreVersion), Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'>
	    <Private>True</Private>"


    let replacements = 
        [
            (oldVersion, newVersion)
            (oldImport, newImport)
            (oldInclude, newInclude)
        ]

    // Creating infix equality and inequality operators
    let regex s = new Regex(s)


    let (=~) s (re: Regex) = re.IsMatch(s)
    let (<>~) s (re: Regex) = not (s =~ re)


    let listDirectories d =
        try
            (new DirectoryInfo(d)).EnumerateDirectories()
            |> Seq.toList
        with
        e -> e.ToString() |> eprintfn "%s"
             []
    

    let listFiles d =
        try
            (new DirectoryInfo(d)).EnumerateFiles()
            |> Seq.toList
        with
        e -> e.ToString() |> eprintfn "%s"
             []


    let filesWithExt ext dir =
        let filter (f: FileInfo) = (f.Extension) =~ (ext |> regex)
        let rec listAll dirs files =
            match dirs with
            | [] -> files
            | d::tail ->
                d |> listFiles
                  |> List.filter filter
                  |> List.append files
                  |> listAll (d |> listDirectories |> List.map(fun d -> d.FullName))
                  |> listAll tail

        listAll [dir] []



    let stringToDocument s = 
        let doc = new XmlDocument()
        doc.LoadXml(s)
        doc


    let stringToNode s  = s |> stringToDocument :> XmlNode


    let fileToXml (file: FileInfo) = 
        use stream = file.OpenText()
        stream.ReadToEnd()
        |> stringToDocument


    let xmlToFile (doc: XmlDocument) (file: FileInfo) =
        use stream = file.OpenWrite()
        doc.Save(stream)


    let findNodeWithChildNode name (doc: XmlDocument) =
    
        doc.ChildNodes
        |> Seq.cast<XmlNode>
        |> Seq.find(fun n -> n.Name = name)
        

    let selectSingleNode path (doc: XmlNode) = 
        try
            let node = doc.SelectSingleNode(path)
            if node = null then None else node |> Some
        with
        _ -> None


    let findNode (node: XmlNode) (doc: XmlDocument) =
        let text = node.InnerXml.Substring(0, node.InnerXml.Length - 3)
        let rec find (r: XmlNode) =
            if r.InnerXml.StartsWith(text) then 
                printfn "Found: %s\nNode: %s" text r.InnerXml
                r |> Some
            else 
                match 
                    r.ChildNodes
                    |> Seq.cast<XmlNode>
                    |> Seq.map find
                    |> Seq.toList with
                | [] -> None
                | h::_ -> h 

        find doc

    let backupFile (file: FileInfo) = 
        new FileInfo(file.DirectoryName + "/" + file.Name + ".backup")


    [<EntryPoint>]
    let main argv =
    
        let files = 
            let dir =
                match argv |> Array.toList with
                | []     -> "."
                | dir::_ -> dir

            printfn "Fixing fsproj files in: %s" dir
            dir |> filesWithExt "fsproj"
                |> List.map (fun f -> printfn "%s" f.Name; f)
                |> List.iter(fun f -> 
                    let bu = f |> backupFile
                    printfn "Created backup: %s" bu.Name
                    let xdoc = f |> fileToXml
                    replacements
                    |> List.iter(fun (o, n) ->
                        xdoc.InnerXml <- xdoc.InnerXml.Replace(o, n))
                    printfn "Processed: %s" f.Name
                    f |> xmlToFile xdoc
                    )
                            
    

        Console.ReadKey() |> ignore

        0 // return an integer exit code
