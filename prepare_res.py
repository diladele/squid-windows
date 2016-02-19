#!/usr/bin/env python 
import sys
import os
import platform
import argparse
import shutil
import glob

#
#
#
class copier:

    def __init__(self, src, dest, sys32):
        self.src = os.path.abspath(src)
        self.dest = os.path.abspath(dest)
        if sys32 is None or sys32 == '':
            self.sys32 = os.path.abspath("c:\\Windows\\System32\\")
        else:
            self.sys32 = sys32

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
            "cygcrypto-1.0.0.dll",
            "cygexpat-1.dll",
            "cyggcc_s-seh-1.dll",
            "cyggssapi_krb5-2.dll",
            "cygiconv-2.dll",
            "cygintl-8.dll",
            "cygk5crypto-3.dll",
            "cygkrb5-3.dll",
            "cygkrb5support-0.dll",
            "cygltdl-7.dll",
            "cygssl-1.0.0.dll",
            "cygstdc++-6.dll",
            "cygwin1.dll",
            "cygxml2-2.dll",
            "cygz.dll",
            "cyglber-2-4-2.dll",
            "cygldap-2-4-2.dll",
            "cygsasl2-3.dll",
            "cygcrypt-0.dll",
            "cygdb-5.3.dll",
			"cyglzma-5.dll"
        ]

        for dll in required_dlls:
            full_path = os.path.join(from_bin,dll);
            if os.path.isfile(full_path):
                shutil.copy2(full_path, target_dir)
                
    def copy_certificates(self, rootfolder):
        # certificates to usr ssl
        cert_from = os.path.join(self.src, "etc", "pki", "ca-trust", "extracted", "pem", "tls-ca-bundle.pem")
        cert_to = os.path.join(self.dest, rootfolder, "ssl", "cert.pem")
        os.makedirs(os.path.join(self.dest, rootfolder, "ssl"))
        shutil.copy2(cert_from, cert_to);
        
        ca_bundle_from = os.path.join(self.src, "etc", "pki", "ca-trust", "extracted", "pem", "tls-ca-bundle.pem")
        ca_bundle_to = os.path.join(self.dest, rootfolder, "ssl", "certs", "ca-bundle.crt")
        os.makedirs(os.path.join(self.dest, rootfolder, "ssl", "certs"))
        shutil.copy2(ca_bundle_from, ca_bundle_to);
        
        ca_bundle_trust_from = os.path.join(self.src, "etc", "pki", "ca-trust", "extracted", "openssl", "ca-bundle.trust.crt")
        ca_bundle_trust_to = os.path.join(self.dest, rootfolder, "ssl", "certs", "ca-bundle.trust.crt")
        shutil.copy2(ca_bundle_trust_from, ca_bundle_trust_to);
                
    def copy_squid(self):
        if os.path.exists(self.dest):
            shutil.rmtree(self.dest)
        os.makedirs(self.dest)
        
        # copy etc\squid
        from_etc_squid = os.path.join(self.src, "etc", "squid")
        to_etc_squid = os.path.join(self.dest, "etc", "squid")
        shutil.copytree(from_etc_squid, to_etc_squid)

        # copy lib\squid
        from_lib_squid = os.path.join(self.src, "lib", "squid")
        to_lib_squid = os.path.join(self.dest, "lib", "squid")
        shutil.copytree(from_lib_squid, to_lib_squid)
        
        # copy usr\share\squid
        from_usr_share_squid = os.path.join(self.src, "usr", "share", "squid")
        to_usr_share_squid = os.path.join(self.dest, "usr", "share", "squid")
        shutil.copytree(from_usr_share_squid, to_usr_share_squid)
        
        # create var\cache\squid
        to_var_cache_squid = os.path.join(self.dest, "var", "cache", "squid")
        os.makedirs(to_var_cache_squid);
        
        # create var\log\squid
        to_var_log_squid = os.path.join(self.dest, "var", "log", "squid")
        os.makedirs(to_var_log_squid);
        
        # create var\run\squid
        to_var_run_squid = os.path.join(self.dest, "var", "run", "squid")
        os.makedirs(to_var_run_squid);
        to_var_run_squid_run_squid = os.path.join(self.dest, "var", "run", "squid", "run", "squid")
        os.makedirs(to_var_run_squid_run_squid);
        
        # copy usr\sbin\squid
        squid_exe_from = os.path.join(self.src, "usr", "sbin", "squid", "squid.exe")
        squid_exe_to = os.path.join(self.dest, "bin", "squid.exe")
        os.makedirs(os.path.join(self.dest, "bin"));
        shutil.copy2(squid_exe_from, squid_exe_to);

        # copy usr\sbin\squidclient
        squid_client_exe_from = os.path.join(self.src, "bin", "squid", "squidclient.exe")
        squid_client_exe_to = os.path.join(self.dest, "bin", "squidclient.exe")
        shutil.copy2(squid_client_exe_from, squid_client_exe_to);

        # copy usr\sbin\purge
        purge_exe_from = os.path.join(self.src, "bin", "squid", "purge.exe")
        purge_exe_to = os.path.join(self.dest, "bin", "purge.exe")
        shutil.copy2(purge_exe_from, purge_exe_to);
        
        # copy configuration
        squid_conf_from = os.path.join("staticres", "squid.conf")
        squid_conf_from_diladele = os.path.join("staticres", "squid.conf.diladele")
        squid_conf_from_settings = os.path.join("staticres", "settings.json")
        license = os.path.join("staticres", "LICENSE")
        cygwin_license = os.path.join("staticres", "CYGWIN_LICENSE")
        credits = os.path.join("staticres", "CREDITS")
        contributors = os.path.join("staticres", "CONTRIBUTORS")

        squid_conf_to = os.path.join(self.dest, "etc", "squid", "squid.conf")
        squid_conf_to_diladele = os.path.join(self.dest, "etc", "squid", "squid.conf.diladele")
        squid_conf_to_settings = os.path.join(self.dest, "bin", "settings.json")
        license_to = os.path.join(self.dest, "bin", "LICENSE")
        cygwin_license_to = os.path.join(self.dest, "bin", "CYGWIN_LICENSE")
        credits_to = os.path.join(self.dest, "bin", "CREDITS")
        contributors_to = os.path.join(self.dest, "bin", "CONTRIBUTORS")

        shutil.copy2(squid_conf_from, squid_conf_to);
        shutil.copy2(squid_conf_from_diladele, squid_conf_to_diladele);
        shutil.copy2(squid_conf_from_settings, squid_conf_to_settings);
        shutil.copy2(license, license_to);
        shutil.copy2(cygwin_license, cygwin_license_to);
        shutil.copy2(credits, credits_to);
        shutil.copy2(contributors, contributors_to);

        #create shared memory folder
        os.makedirs(os.path.join(self.dest, "dev/shm"));

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
