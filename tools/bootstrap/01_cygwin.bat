@echo off

::
:: this script downloads latest cygwin into the bin folder
::

:: configure folders
set MYDIR=%~dp0
set ROOT=%MYDIR%..\..\
set BIN=%ROOT%bin

:: create the bin folder
if not exist %BIN% mkdir %BIN%

:: download cygwin (curl is part of Windows)
curl -L -o %BIN%\setup-x86_64.exe https://cygwin.com/setup-x86_64.exe

:: and wait let admin check the output
echo "SUCCESS: run next step please!"
pause
