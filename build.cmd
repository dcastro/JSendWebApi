.nuget\nuget.exe "Install" "xunit.runner.console" -OutputDirectory packages -ExcludeVersion -version 2.0.0
.nuget\NuGet.exe "Install" "FAKE" -OutputDirectory packages -ExcludeVersion -version 3.17.14
packages\FAKE\tools\Fake.exe build.fsx %* encoding=utf-8
