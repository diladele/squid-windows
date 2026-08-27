@echo off

::
:: this script downloads squid sources into the build folder
::

:: configure folders
set MYDIR=%~dp0
set ROOT=%MYDIR%..\..\
set BIN=%ROOT%bin
set BUILD=%ROOT%ztmp

:: configure squid versions
set MAJOR=7
set MINOR=7

:: download latest squid into the build folder
curl -L -o %BUILD%\usr\src\squid-%MAJOR%.%MINOR%.tar.gz https://github.com/squid-cache/squid/releases/download/SQUID_%MAJOR%_%MINOR%/squid-%MAJOR%.%MINOR%.tar.gz

:: copy the patch into build folder
copy tools.cc.patch %BUILD%\usr\src\

:: unpack it into src folder in cygwin
%BUILD%\bin\bash.exe -lc "tar -xzf /usr/src/squid-%MAJOR%.%MINOR%.tar.gz -C /usr/src/"

:: apply the patch
%BUILD%\bin\bash.exe -lc "patch /usr/src/squid-%MAJOR%.%MINOR%/src/tools.cc </usr/src/tools.cc.patch"

:: and let admin check the output
echo "SUCCESS: run next step please!"
pause
