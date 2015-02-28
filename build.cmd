.nuget\nuget.exe "Install" "xunit.runners" -OutputDirectory packages -ExcludeVersion -version 1.9.2
.nuget\NuGet.exe "Install" "FAKE" -OutputDirectory packages -ExcludeVersion
packages\FAKE\tools\Fake.exe build.fsx %* encoding=utf-8
