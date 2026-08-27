@echo off

:: configure folders
set MYDIR=%~dp0
set ROOT=%MYDIR%..\..
set DEST=%ROOT%\res

:: create folder to store temporary *.wixobj files - note it MUST end with a slash for candle to work
set OUT=%ROOT%\ztmp\msi\

:: re make it
if exist %OUT% (
    rmdir /s /q %OUT%
)
mkdir %OUT%

:: copy msi assets
copy %ROOT%\src.pack\msi\assets\banner.bmp %OUT%
copy %ROOT%\src.pack\msi\assets\dialog.bmp %OUT%
copy %ROOT%\src.pack\msi\assets\SquidIcon.ico %OUT%
copy %ROOT%\src.pack\msi\assets\license.rtf %OUT%

:: set variables assuming wix 3.11 is installed
set WIX=%ROOT%\contrib\wix\3.11
set CANDLE=%WIX%\CANDLE.EXE
set LIGHT=%WIX%\LIGHT.EXE
set HEAT=%WIX%\HEAT.EXE

:: run candle to compile wsx files into wixobj
%CANDLE% /nologo /arch x64 /pedantic ^
	/ext WiXUtilExtension /ext WixUIExtension /ext WixDifxAppExtension /ext WixFirewallExtension /ext WixNetFxExtension ^
	/dResDir=%DEST% ^
	/out %OUT% ^
	%ROOT%\src.pack\msi\*.wxs

:: and then call light to actually make the MSI
%LIGHT% /nologo /pedantic ^
	/ext WiXUtilExtension /ext WixUIExtension /ext WixDifxAppExtension /ext WixFirewallExtension /ext WixNetFxExtension ^
	/out %ROOT%\acceptance\squid-7.7_amd64.msi ^
	/cultures:en-us ^
	%OUT%\*.wixobj

:: and let admin check the output
echo "SUCCESS: run next step please!"
pause
