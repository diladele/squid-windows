#!/usr/bin/env python 
import sys
import os
import platform
import argparse
import shutil
import glob
import ctypes
import ntpath

__CSL = None
def symlink(source, link_name):
    global __CSL
    if __CSL is None:
        import ctypes
        csl = ctypes.windll.kernel32.CreateSymbolicLinkW
        csl.argtypes = (ctypes.c_wchar_p, ctypes.c_wchar_p, ctypes.c_uint32)
        csl.restype = ctypes.c_ubyte
        __CSL = csl
    flags = 0
    if source is not None and os.path.isdir(source):
        flags = 1
    if __CSL(link_name, source, flags) == 0:
        raise ctypes.WinError()

def correct_symbolic_links(error_directory):
        target_dir = os.path.abspath(error_directory)

        directories = []
        # copy all windows cygwin dlls
        files = list(glob.iglob(os.path.join(target_dir, "*")))
        for file in files:
            if os.path.isdir(file):
                directories.append(file)
        
        for file in files:
            if os.path.isfile(file):
                prefix = ntpath.basename(file).split("-", 1)[0]
                for dir in directories:
                    dirprefix = ntpath.basename(dir).split("-", 1)[0]
                    if dirprefix == prefix:
                        print dirprefix + " " + prefix
                        os.remove(file)
                        symlink(dir, file)
                        break
#
# code
#
def main():
    parser = argparse.ArgumentParser(description='Builds Squid target directory structure.')
    parser.add_argument("--errorpages", help="Absolute path to the folder with error pages.", required=True)
    
    args = parser.parse_args()    
    
    correct_symbolic_links(args.errorpages)

#
# entry point
#
main() 