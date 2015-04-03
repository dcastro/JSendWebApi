// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile

RestorePackages()

// Properties
let buildDir = "./build/"
let testResultsDir = "./testresults/"
let nugetDir = buildDir @@ "nuget"
let version = "0.2.0.0"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
    CleanDir nugetDir
    CleanDir testResultsDir
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
    !! "./tests/**/bin/debug/*Tests.dll" 
        |> xUnit (fun p ->
            {p with 
                ShadowCopy = false;
                HtmlOutput = true;
                OutputDir = testResultsDir})
)

Target "CreateNugets" (fun _ ->
    let index = version.LastIndexOf '.' - 1
    let semanticVersion = version.[..index]
    
    let nuspecs = !! "./src/**/*.nuspec"
    
    for nuspec in nuspecs do
        nuspec
            |> NuGet (fun p ->
                {p with
                    Version = semanticVersion
                    OutputPath = nugetDir
                    WorkingDir = buildDir})
)

Target "All" DoNothing

// Dependencies
"Clean"
    ==> "UpdateVersion"
    ==> "Build"
    ==> "BuildTests"
    ==> "RunTests"
    ==> "CreateNugets"
    ==> "All"

// Start build
RunTargetOrDefault "All"
