@echo off

:: configure folders
set MYDIR=%~dp0
set ROOT=%MYDIR%..\..\
set BIN=%ROOT%bin
set BUILD=%ROOT%ztmp

:: configure squid versions
set MAJOR=7
set MINOR=7

:: configure
%BUILD%\bin\bash.exe -lc "cd /usr/src/squid-%MAJOR%.%MINOR% && ./configure --bindir=/bin/squid --sbindir=/usr/sbin/squid --sysconfdir=/etc/squid --datadir=/usr/share/squid --libexecdir=/usr/lib/squid --disable-strict-error-checking --with-logdir=/var/log/squid --with-swapdir=/var/cache/squid --with-pidfile=/var/run/squid.pid --enable-ssl --enable-delay-pools --enable-ssl-crtd --enable-icap-client --enable-esi --disable-eui --localstatedir=/var/run/squid --sharedstatedir=/var/run/squid --datarootdir=/usr/share/squid --enable-disk-io='AIO,Blocking,DiskThreads,IpcIo,Mmapped' --enable-auth-basic='DB,LDAP,NCSA,POP3,RADIUS,SASL,SMB,fake,getpwnam' --enable-auth-ntlm='fake' --enable-auth-negotiate='kerberos,wrapper' --enable-external-acl-helpers='LDAP_group,SQL_session,eDirectory_userip,file_userip,kerberos_ldap_group,session,unix_group,wbinfo_group' --with-openssl --with-filedescriptors=65536 --enable-removal-policies='lru,heap'"

::
:: --enable-external-acl-helpers= - removed time_quota as it is required but cannot be built
::

:: and let admin check the output
echo "SUCCESS: run next step please!"
pause
