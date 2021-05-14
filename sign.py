import os
import argparse
import subprocess


#
#
#
class signer:

    def __init__(self):

        self.signtool = "c:/program files (x86)/windows kits/10/bin/10.0.19041.0/x64/signtool.exe"


    def sign(self, file):

        if not os.path.isfile(file):
            raise Exception("file " + file + " does not exist!") 

        cmd = " ".join(
            [
                self.signtool, 
                "sign",
                "/debug",
                "/a",
                "/tr",
                "http://rfc3161timestamp.globalsign.com/advanced",
                "/td",
                "SHA256",
                "\"" + os.path.abspath(file) + "\""
            ]
        )
        if 0 != subprocess.call(cmd) :
            raise Exception("can not sign file " + cmd) 

    def verify(self, file):

        if not os.path.isfile(file):
            raise Exception("file " + file + " does not exist!") 

        cmd = " ".join(
            [
                self.signtool, 
                "verify",
                "/debug",
                "/v",
                "/pa",
                "\"" + os.path.abspath(file) + "\""
            ]
        )
        if 0 != subprocess.call(cmd) :
            raise Exception("can not verify file " + cmd) 

def main():
    path = "m:\\squid-windows\\bin\\x64\\Debug\\squid.msi"

    signer().sign(path)
    signer().verify(path)



#
#
#
main()
