# Squid for Windows

> Squid is a caching proxy for the Web supporting HTTP, HTTPS, FTP, and more. It reduces bandwidth and improves response times by caching and reusing frequently-requested web pages. Squid has extensive access controls and makes a great server accelerator. It runs on most available operating systems, including Windows and is licensed under the GNU GPL.
> <cite> <http://www.squid-cache.org>

This project provides MSI installer package for Squid proxy server which can run on Microsoft Windows. It enables Squid installation with just a few simple clicks. Current version is based on the latest Squid 7.7 build using Cygwin Windows 64 bit.

## Installation

* Download [Squid 7.7 for Windows MSI installer](http://www.diladele.com/squid/download.html) from Diladele B.V.
* Run it and click through *Next* buttons till the product is installed.

## Secure Web Gateway for Microsoft Windows

If you are looking for functionality beyond what the console version of Squid provides, consider exploring our secure web gateway solution for Microsoft Windows, [Web Filtering Proxy](https://www.diladele.com/webproxy/). 

Web Filtering Proxy is a Windows native web filtering solution that seamlessly integrates with Active Directory and provides rich content and web filtering functionality to sanitize Internet traffic passing into home/enterprise networks. It may be used to block illegal or potentially malicious file downloads, remove annoying advertisements, prevent access to various categories of web sites and block resources with adult/explicit content.

The MSI is available from [download page](https://www.diladele.com/webproxy/download.html) or can be installed using winget package manager by running `winget install webproxy`. Administrator guide and various tutorials [are available online](https://www.diladele.com/webproxy/docs/). If you are running a home or small business network the product is also [completely free](https://www.diladele.com/webproxy/licensing.html).

## How to Build

Follow these steps to build your own Squid for Windows after checking out this repository.

### Step 1. Install Cygwin

To install Cygwin into `ztmp` folder, open terminal console, change into the `tools/bootstrap/` folder and run `01_cygwin.bat` followed by `02_packages.bat`. After that you should have Cygwin environment installed and ready.

### Step 2. Build Squid

To build Squid 7.7, first make sure the `ztmp/usr/src` folder is empty, then open terminal console, change into the `tools/squid/` folder and run `01_download.bat`, `02_configure.bat`, `03_compile.bat` and finally `04_deploy.bat` files. After that you should have built Squid binaries in your `ztmp` cygwin folder.

### Step 3. Build Tools

Now we need to build some tools that make the integration easier. Run `01_csharp.bat` in the `tools/build` folder. If you wish to sign those, also run `02_sign.bat`. Be sure to change the SHA of the certificate used for signing to your own certificate.

### Step 3. Pack MSI

Next step is to pack the MSI file. First we need to take only necessary files from the cygwin folder structure in `ztmp` and add some system Windows files. Open terminal console again, navigate to `tools/msi` folder and run `01_resources.bat`. This will create Squid folder structure in `res` folder. 

Run `02_msi.bat` to make the MSI, this will be created in `acceptance` folder. If you need to sign it also run `02_sign.bat`.

## Help and Support

Finally try installing your MSI to see if all works as expected. If not Squid documentation can be found at http://www.squid-cache.org. In case of any errors in the *installer only*, please send an email to support@diladele.com or post your question in the [discussion group](https://github.com/diladele/squid-windows/discussions). For squid specific questions please use one of Squid mailing lists http://www.squid-cache.org/Support/mailing-lists.html.

## Credits

We admire people working on Squid Cache server, who deliver great product to all of us.
