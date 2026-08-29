@echo off

:: configure folders
set MYDIR=%~dp0
set ROOT=%MYDIR%..\..

:: use sigtool from Windows 10 SDK
set SIGN="c:/Program Files (x86)/Windows Kits/10/bin/10.0.26100.0/x64/signtool.exe"

:: sign the binaries with our certificate
%SIGN% sign ^
	/debug ^
	/sha1 53753D12EA58FBF20C205169839D8CF0D2318A02 ^
	/fd SHA256 ^
	/td SHA256 ^
	/tr http://timestamp.globalsign.com/tsa/r45standard ^
	%ROOT%\bin\amd64\release\Diladele.Squid.Tray.exe ^
	%ROOT%\bin\amd64\release\Diladele.Squid.Service.exe

:: and verify those
%SIGN% verify /debug /v /pa %ROOT%\bin\amd64\release\Diladele.Squid.Tray.exe
%SIGN% verify /debug /v /pa %ROOT%\bin\amd64\release\Diladele.Squid.Service.exe

:: let admin check the output
echo "SUCCESS: run next step please!"
pause
