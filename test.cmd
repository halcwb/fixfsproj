@echo off
cls
if not exist "%~dp0\src\packages\FAKE\tools\Fake.exe" "%~dp0\src\.nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "%~dp0src\packages" "-ExcludeVersion"
"%~dp0\src\packages\FAKE\tools\Fake.exe" %~dp0\src\test.fsx
