// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake

RestorePackages()

// Properties
let buildDir = "./build/"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "Build" (fun _ ->
    !! "src/JSend.WebApi/JSend.WebApi.csproj"
        |> MSBuildRelease buildDir "Build"
        |> Log "Build-Output: "
)

Target "Default" DoNothing

// Dependencies
"Clean"
    ==> "Build"
    ==> "Default"

// Start build
RunTargetOrDefault "Default"
