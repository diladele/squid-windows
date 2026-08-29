@echo off

:: configure folders
set MYDIR=%~dp0
set ROOT=%MYDIR%..\..\
set BIN=%ROOT%bin
set BUILD=%ROOT%ztmp

:: configure squid versions
set MAJOR=7
set MINOR=7

:: deploy into sygwin build folder by make install
%BUILD%\bin\bash.exe -lc "cd /usr/src/squid-%MAJOR%.%MINOR% && make install-strip"

:: and let admin check the output
echo "SUCCESS: run next step please!"
pause