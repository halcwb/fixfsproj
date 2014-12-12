#r @"packages\FAKE\tools\FakeLib.dll"

//System.IO.Directory.SetCurrentDirectory (__SOURCE_DIRECTORY__)

open Fake

let dir = @"."

Target "Start" (fun _ ->
    trace "Started building fixfsproj solution"
)


Target "Build TestFixFsProj" (fun _ -> 
    !! (dir + "/**/*.fsproj")
       |> Seq.map (fun d -> printfn "Found: %s" d; d)
       |> MSBuildRelease ("./" + dir + "/bin/Release") "Build"
       |> Log "TestFixFsProj output:"
)


Target "Run Tests" (fun _ ->
    let dirs = 
        !! (dir + "/**/bin/Release/TestFixFsproj.dll")
        |> Seq.take 1
    printfn "Found %d test dll's" (dirs |> Seq.length)
    dirs
       |> NUnit (fun p ->
          { p with
              DisableShadowCopy = true;
              OutputFile = dir + "TestResults.xml" })
)


Target "Finish" (fun _ -> 
    trace "Finished building fixfsproj solution"
)

"Start"
==> "Build TestFixFsProj"
==> "Run Tests"
==> "Finish"

RunTargetOrDefault "Finish"     
