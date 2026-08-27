#!/usr/bin/env python
import sys
import os
import platform
import argparse
import shutil
import glob
import ntpath

#
# helper for cygwin path conversion
#
def cygwin_path(win_path):

    # resolve it
    path = ntpath.normpath(win_path)

    # convert Windows drive notation to Cygwin notation
    drive, rest = ntpath.splitdrive(path)
    cygpath = "/cygdrive/" + drive[0].lower() + rest.replace("\\", "/")

    # ok then
    return cygpath


#
#
#
class copier:

    def __init__(self, src, dest, sys32):

        self.src  = cygwin_path(src)
        self.dest = cygwin_path(dest)
        
        if not sys32:
            self.sys32 = cygwin_path("c:\\Windows\\System32\\")
        else:
            self.sys32 = sys32

        # recreate the destination folder
        if os.path.exists(self.dest):
            shutil.rmtree(self.dest)

        os.makedirs(self.dest)

    def copy_cygwin(self, target_dir):
        
        # copy all windows cygwin dlls
        files = list(glob.iglob(os.path.join(self.sys32, "api-ms-win-core-*.dll")))
        files.extend(list(glob.iglob(os.path.join(self.sys32, "api-ms-win-security-*.dll"))))
        for file in files:
            if os.path.isfile(file):
                shutil.copy2(file, target_dir)

        # copy all cygwin dlls
        from_bin = os.path.join(self.src, "bin")
        required_dlls = [
            "cygcom_err-2.dll",
            "cygcrypto-1.1.dll",
            "cygexpat-1.dll",
            "cyggcc_s-seh-1.dll",
            "cyggssapi_krb5-2.dll",
            "cygiconv-2.dll",
            "cygintl-8.dll",
            "cygk5crypto-3.dll",
            "cygkrb5-3.dll",
            "cygkrb5support-0.dll",
            "cygltdl-7.dll",
            "cygssl-1.1.dll",
            "cygstdc++-6.dll",
            "cygwin1.dll",
            "cygxml2-2.dll",
            "cygz.dll",
            "cyglber-2-4-2.dll",
            "cygldap-2-4-2.dll",
            "cygsasl2-3.dll",
            "cygcrypt-0.dll",
            "cygdb-5.3.dll",
            "cyglzma-5.dll",
            "cygcrypto-3.dll",
            "cygssl-3.dll"
        ]

        for dll in required_dlls:
            full_path = os.path.join(from_bin,dll);
            if os.path.isfile(full_path):
                shutil.copy2(full_path, target_dir)

    def copy_certificates(self, rootfolder):

        # certificates to usr ssl
        cert_from = os.path.join(self.src, "etc", "pki", "ca-trust", "extracted", "pem", "tls-ca-bundle.pem")
        cert_to   = os.path.join(self.dest, rootfolder, "ssl", "cert.pem")
        os.makedirs(os.path.join(self.dest, rootfolder, "ssl"))
        shutil.copy2(cert_from, cert_to);

        ca_bundle_from = os.path.join(self.src, "etc", "pki", "ca-trust", "extracted", "pem", "tls-ca-bundle.pem")
        ca_bundle_to   = os.path.join(self.dest, rootfolder, "ssl", "certs", "ca-bundle.crt")
        os.makedirs(os.path.join(self.dest, rootfolder, "ssl", "certs"))
        shutil.copy2(ca_bundle_from, ca_bundle_to);

        ca_bundle_trust_from = os.path.join(self.src, "etc", "pki", "ca-trust", "extracted", "openssl", "ca-bundle.trust.crt")
        ca_bundle_trust_to   = os.path.join(self.dest, rootfolder, "ssl", "certs", "ca-bundle.trust.crt")
        shutil.copy2(ca_bundle_trust_from, ca_bundle_trust_to);

    def copy_squid(self):

        # copy etc/squid
        shutil.copytree(
            os.path.join(self.src, "etc", "squid"), 
            os.path.join(self.dest, "etc", "squid")
        )

        # copy lib/squid
        shutil.copytree(
            os.path.join(self.src, "lib", "squid"), 
            os.path.join(self.dest, "lib", "squid")
        )

        # copy usr/share/squid
        shutil.copytree(
            os.path.join(self.src, "usr", "share", "squid"), 
            os.path.join(self.dest, "usr", "share", "squid")
        )

        # create var/cache/squid
        os.makedirs(os.path.join(self.dest, "var", "cache", "squid"))

        # create var/log/squid
        os.makedirs(os.path.join(self.dest, "var", "log", "squid"))

        # create var/run/squid
        os.makedirs(os.path.join(self.dest, "var", "run", "squid"))
        os.makedirs(os.path.join(self.dest, "var", "run", "squid", "run", "squid"))

        # copy usr/sbin/squid
        os.makedirs(os.path.join(self.dest, "bin"));
        shutil.copy2(
            os.path.join(self.src, "usr", "sbin", "squid", "squid.exe"), 
            os.path.join(self.dest, "bin", "squid.exe")
        )

        ## removed in squid 7
        # copy usr\sbin\squidclient
        #squid_client_exe_from = os.path.join(self.src, "bin", "squid", "squidclient.exe")
        #squid_client_exe_to = os.path.join(self.dest, "bin", "squidclient.exe")
        #shutil.copy2(squid_client_exe_from, squid_client_exe_to);
        # copy usr\sbin\purge
        #purge_exe_from = os.path.join(self.src, "bin", "squid", "purge.exe")
        #purge_exe_to = os.path.join(self.dest, "bin", "purge.exe")
        #shutil.copy2(purge_exe_from, purge_exe_to);

        # create shared memory folder
        os.makedirs(os.path.join(self.dest, "dev/shm"))

        # certificates to usr
        self.copy_certificates("usr")
        self.copy_certificates("etc")

#
# code
#
def main():
    
    parser = argparse.ArgumentParser(description='Builds Squid target directory structure.')
    parser.add_argument("--src", help="Absolute path of the root of cygwin installation where Squid is installed.", required=True)
    parser.add_argument("--dest", help="Directory where Squid directory structure will be reproduced.", required=True)
    #parser.add_argument("--assets", help="Directory where MSI assets are stored.", required=True)
    parser.add_argument("--sys32", help="Path to system32 windows folder.", required=False)

    args = parser.parse_args()

    c = copier(args.src, args.dest, args.sys32)
    c.copy_squid()

    target_dir = os.path.join(args.dest, "bin")
    c.copy_cygwin(target_dir)

    target_dir = os.path.join(args.dest, "lib/squid")
    c.copy_cygwin(target_dir)

#
# entry point
#
main()
