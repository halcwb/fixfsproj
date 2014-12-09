﻿#r @"packages\FAKE\tools\FakeLib.dll"

open Fake

open Fake

let dir = @"src/TestFixFsProj"

Target "Start" (fun _ ->
    trace "Started building fixfsproj solution"
)

Target "Build Utils" (fun _ -> 
    !! (dir + "/**/*.fsproj")
       |> MSBuildRelease ("./" + dir + "/bin/Release") "Build"
       |> Log "TestFixFsProj output:"
)

Target "Finish" (fun _ -> 
    trace "Finished building fixfsproj solution"
)

"Start"
==> "Build Utils"
==> "Finish"

RunTargetOrDefault "Finish"     
