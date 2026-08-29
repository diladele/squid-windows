@echo off

::
:: this script installs required cygwin packages into the build folder
::

:: configure folders
set MYDIR=%~dp0
set ROOT=%MYDIR%..\..\
set BIN=%ROOT%bin
set BUILD=%ROOT%ztmp

:: change to bin folder
cd %BIN%

:: run cygwin and install all packages
%BIN%\setup-x86_64.exe -q ^
	--no-admin ^
	-R %BUILD% ^
	-s https://ftp.snt.utwente.nl/pub/software/cygwin/ ^
    -P git,make,patch,make,automake,autoconf,gcc-g++,gdb,libtool ^
    -P cppunit ^
    -P libcom_err-devel ^
    -P libcrypt-devel ^
    -P libdb-devel ^
    -P libexpat-devel ^
    -P libkrb5-devel ^
    -P libiconv-devel ^
    -P openldap-devel ^
    -P libsasl2-devel ^
    -P libxml2-devel ^
    -P perl-DBI

:: change back to the tools folder
cd %MYDIR%

:: and let admin check the output
echo "SUCCESS: run next step please!"
pause

