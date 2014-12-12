@echo off
cls
if not exist "%~dp0\src\tools\Nunit\nunit-console.exe" "%~dp0\src\.nuget\NuGet.exe" "Install" "Nunit" "-OutputDirectory" "%~dp0src\tools" "-ExcludeVersion"
if not exist "%~dp0\src\tools\Nunit\nunit-console.exe" "%~dp0\src\.nuget\NuGet.exe" "Install" "NUnit.Runners.Net4" "-OutputDirectory" "%~dp0src\tools" "-ExcludeVersion"
"%~dp0\src\packages\FAKE\tools\Fake.exe" %~dp0\src\test.fsx
