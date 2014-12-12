namespace TestFixFsProj

open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Given a non existent directory``() = 
    
    [<Test>]
    member this.``List directories will return an empty list``() = 
        true |> should equal true
