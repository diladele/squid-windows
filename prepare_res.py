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

    def copy_cygwin(self):
        target_dir = os.path.join(self.dest, "bin")

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
            "cygz.dll"
        ]

        for dll in required_dlls:
            full_path = os.path.join(from_bin,dll);
            if os.path.isfile(full_path):
                shutil.copy2(full_path, target_dir)

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
        
        # copy usr\sbin\squid
        squid_exe_from = os.path.join(self.src, "usr", "sbin", "squid.exe")
        squid_exe_to = os.path.join(self.dest, "bin", "squid.exe")
        os.makedirs(os.path.join(self.dest, "bin"));
        shutil.copy2(squid_exe_from, squid_exe_to);

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
    c.copy_cygwin()

#
# entry point
#
main()
