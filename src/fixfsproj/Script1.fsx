#load "Program.fs"

open Informedica.fixfsproj.Fix

open System.IO

let fsproj = 
    filesWithExt "fsproj" currDir
    |> Seq.cast<FileInfo>
    |> Seq.find(fun f -> f.Name.StartsWith("TestFix"))


let xdoc = fsproj |> fileToXml

let backupFile (file: FileInfo) = 
    new FileInfo(file.DirectoryName + "/" + file.Name + ".backup")
    
let backup = fsproj |> backupFile

backup
|> xmlToFile xdoc

replacements
|> List.iter(fun (o, n) ->
    xdoc.InnerXml <- xdoc.InnerXml.Replace(o, n))

fsproj
|> xmlToFile xdoc 
