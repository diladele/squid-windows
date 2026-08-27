@echo off

:: configure folders
set MYDIR=%~dp0
set ROOT=%MYDIR%..\..
set BUILD=%ROOT%\ztmp
set DEST=%ROOT%\res

:: recreate complete squid structure to be packed into msi
%BUILD%\bin\python3.9.exe resources.py --src=%BUILD% --dest=%DEST%

:: copy additional assets
copy %ROOT%\src.pack\msi\assets\squid.conf %DEST%\etc\squid\squid.conf
copy %ROOT%\src.pack\msi\assets\settings.json %DEST%\bin\settings.json
copy %ROOT%\src.pack\msi\assets\LICENSE %DEST%\bin\LICENSE
copy %ROOT%\src.pack\msi\assets\CYGWIN_LICENSE %DEST%\bin\CYGWIN_LICENSE
copy %ROOT%\src.pack\msi\assets\CREDITS %DEST%\bin\CREDITS
copy %ROOT%\src.pack\msi\assets\CONTRIBUTORS %DEST%\bin\CONTRIBUTORS

:: configure path to heat
set HEAT=%ROOT%\contrib\wix\3.11\HEAT.EXE

:: harvest squid resources for MSI into installer folder - not optimal
%HEAT% dir %DEST% -dr INSTALLFOLDER -cg SquidFilesGroup ^
	-gg -g1 -sf -srd -ke -sw5150 -nologo -suid -var "var.ResDir" ^
	-out %ROOT%\src.pack\msi\SquidResourceFiles.wxs

:: copy C# tools to destination too
xcopy "%ROOT%\bin\amd64\release\*" "%DEST%\bin\" /E /I /Y

:: TODO reconstruct error pages folder
::%BUILD%\bin\python3.9.exe errors.py --errorpages=%BUILD%\usr\src\squid-7.7\errors
::%DEST%\usr\share\squid\errors

:: and let admin check the output
echo "SUCCESS: run next step please!"
pause
