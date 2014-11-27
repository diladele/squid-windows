import os
import argparse
import subprocess

def sign(file, pfx):
    
    cmd = " ".join(
        [
            "c:/Program Files (x86)/Windows Kits/8.0/bin/x64/signtool.exe", 
            "sign", 
            "/ph", 
            "/v", 
            #"/ac " + os.path.abspath("contrib/certificates/production/After_10-10-10_MSCV-VSClass3.cer"), 
            "/f " + pfx, 
            "/p " + os.environ['DILADELE_B_V_CERTIFICATE_PASSWORD'], 
            "/d", '"Diladele Web Safety"', 
            "/t", "http://timestamp.verisign.com/scripts/timestamp.dll", "\"" + os.path.abspath(file) + "\"" 
        ]
    )
    if 0 != subprocess.call(cmd) :
        raise Exception("can not sign file " + cmd) 

#
# code
#
def main():
    parser = argparse.ArgumentParser(description='Signs MSI installer.')
    parser.add_argument("--msi", help="full path to MSI to sign", required=True)
    parser.add_argument("--pfx", help="full path to PFX encoded certificate", required=True)
    
    args = parser.parse_args()    
    sign(args.msi, args.pfx)

#
# entry point
#
main()
