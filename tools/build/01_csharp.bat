@echo off

:: configure folders
set MYDIR=%~dp0
set ROOT=%MYDIR%..\..

:: use msbuild for csharp projects for .Net Framework 4.8
set MSBUILD="c:/Program Files/Microsoft Visual Studio/2022/Professional/MSBuild/Current/Bin/amd64/MSBuild.exe"

:: remove artefacts in bin
rmdir /S /Q %ROOT%\bin\amd64\debug
rmdir /S /Q %ROOT%\bin\amd64\release

:: and obj files too
rmdir /S /Q %ROOT%\src.net\squidsrv\obj
rmdir /S /Q %ROOT%\src.net\squidtray\obj

:: build both
%MSBUILD% %ROOT%\src.net\squidsrv\squidsrv.csproj /t:Rebuild /p:Configuration=Release
%MSBUILD% %ROOT%\src.net\squidtray\squidtray.csproj /t:Rebuild /p:Configuration=Release

:: and let admin check the output
echo "SUCCESS: run next step please!"
pause
