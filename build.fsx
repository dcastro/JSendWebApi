// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile

RestorePackages()

// Properties
let buildDir = "./build/"
let testResultsDir = "./testresults/"
let nugetDir = buildDir @@ "nuget"
let nuspec = "./src/JSend.WebApi.nuspec"
let version = "1.0.0.0"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "UpdateVersion" (fun _ ->
    BulkReplaceAssemblyInfoVersions "." (fun f ->
        {f with
            AssemblyVersion = version
            AssemblyFileVersion = version})
)

Target "Build" (fun _ ->
    !! "./src/**/*.csproj"
        |> MSBuildRelease buildDir "Build"
        |> Log "Build-Output: "
)

Target "BuildTests" (fun _ ->
    !! "./tests/**/*.csproj"
        |> MSBuildDebug null "Build"
        |> ignore
)

Target "RunTests" (fun _ ->
    let testAssemblies = !! "./tests/**/bin/debug/*.Tests.dll"
    CleanDir testResultsDir

    testAssemblies 
        |> xUnit (fun p ->
            {p with 
                ShadowCopy = false;
                HtmlOutput = true;
                OutputDir = testResultsDir})
)

Target "CreateNuget" (fun _ ->
    CleanDir nugetDir

    NuGet (fun p ->
            {p with
                Version = version
                OutputPath = nugetDir
                WorkingDir = buildDir})
            nuspec
)

Target "Default" DoNothing

// Dependencies
"Clean"
    ==> "UpdateVersion"
    ==> "Build"
    ==> "BuildTests"
    ==> "RunTests"
    ==> "CreateNuget"
    ==> "Default"

// Start build
RunTargetOrDefault "Default"
