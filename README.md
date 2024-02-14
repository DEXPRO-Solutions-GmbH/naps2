# Squeeze Scanner

This scanner is based in the Otris fork of the NAPS2 scanner.

## Changes Made By DEXPRO

- Hide DOCUMENTS menu points by default (`appsettings.xml`)
- Add Squeeze Upload
- Add Squeeze Settings
- Change the ICON application icon

## How To Build

### 1. Build project in visual studio

There are multiple solution configurations that need to be build:

- InstallerEXE
- InstallerMSI
- Standalone
- Release

For each of these configurations, do the following:

1. Select the solution in Visual Studio
1. Right click "Build solution"

### 2. Package portable and installer

The `NAPS2/setup` folder contains the script `Publish-NAPS2.ps1`. This script seems to be the correct one to build
the project without even having to use Visual Studio.

This will build the installer and the portable versions:

```ps1
cd NAPS2
cd setup
.\Publish-NAPS2.ps1 -Version "1.0.0"
```

The build result `NAPS2/setup/publish`.

# DOCUMENTS Scanner

A fork of NAPS2 (Not Another PDF Scanner 2)

## About DOCUMENTS Scanner

DOCUMENTS Scanner is a fork of NAPS2 with the following improvements:

- New and improved design
- Upload to DOCUMENTS (Simulates QuickDropZone)
- Invoke external tool with PDF (e.g. DOCUMENTS Drop)
- Detect barcodes
- Select batches by barcodes

## Invoke external tools

Under the following registry key you need to set two values. :

- HKEY_LOCAL_MACHINE\SOFTWARE\NAPS2\Send
  - **Target** = [Path to the external tool .exe]
  - **Args** = [Parameters for external tool. `{0}` will be replaced by the path of the pdf file]

Alternativly you can configure this using the appsettings.xml file.

## About NAPS2

NAPS2 is a document scanning application with a focus on simplicity and ease of use. Scan your documents from WIA- and TWAIN-compatible scanners, organize the pages as you like, and save them as PDF, TIFF, JPEG, PNG, and other file formats. Requires .NET Framework 4.0 or higher.

Visit the NAPS2 home page at [www.naps2.com](http://www.naps2.com).

Other links:

- [Documentation](http://www.naps2.com/support.html)
- [Translations](http://translate.naps2.com/) - [Doc](http://www.naps2.com/doc-translations.html)
- [File a Ticket](https://sourceforge.net/p/naps2/tickets/) - For bug reports, feature requests, and general support inquiries.
- [Discussion Forums](https://sourceforge.net/p/naps2/discussion/general/) - For more open-ended discussion.
- [Donate](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=M77MFAP2ZV9RG)

### For developers

Have a look at the [Developer Onboarding](https://www.naps2.com/doc-dev-onboarding.html) page.
