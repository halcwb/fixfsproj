#r @"packages\FAKE\tools\FakeLib.dll"

open Fake

open Fake

let dir = @"src/TestFixFsProj"

Target "Start" (fun _ ->
    trace "Started building fixfsproj solution"
)

Target "Build TestFixFsProj" (fun _ -> 
    !! (dir + "/**/*.fsproj")
       |> MSBuildRelease ("./" + dir + "/bin/Release") "Build"
       |> Log "TestFixFsProj output:"
)

Target "Finish" (fun _ -> 
    trace "Finished building fixfsproj solution"
)

"Start"
==> "Build TestFixFsProj"
==> "Finish"

RunTargetOrDefault "Finish"     
