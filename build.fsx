// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake

RestorePackages()

// Properties
let buildDir = "./build/"
let testResultsDir = "./testresults/"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "Build" (fun _ ->
    !! "src/**/*.csproj"
        |> MSBuildRelease buildDir "Build"
        |> Log "Build-Output: "
)

Target "BuildTests" (fun _ ->
    !! "tests/**/*.csproj"
        |> MSBuildDebug null "Build"
        |> ignore
)

Target "RunTests" (fun _ ->
    let testAssemblies = !! "tests/**/bin/debug/*.Tests.dll"
    CleanDir testResultsDir

    testAssemblies 
        |> xUnit (fun p ->
            {p with 
                ShadowCopy = false;
                HtmlOutput = true;
                OutputDir = testResultsDir})
)

Target "Default" DoNothing

// Dependencies
"Clean"
    ==> "Build"
    ==> "BuildTests"
    ==> "RunTests"
    ==> "Default"

// Start build
RunTargetOrDefault "Default"
