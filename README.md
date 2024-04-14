# WMR100.NET

A .NET library to interface with Oregon Scientific weather stations that are compatible with the WMR100 protocol. Currently the library offers limited support and can only decode `Clock` and `Temperature/Humidity` data. Is uses the [LibUsbDotNet](https://github.com/LibUsbDotNet/LibUsbDotNet) library to handle the USB communication.

[![NuGet Version (WMR100.NET)](https://img.shields.io/nuget/v/WMR100.NET.svg)](https://www.nuget.org/packages/WMR100.NET/)
[![License: GNU GPL v2](https://img.shields.io/badge/License-GPL_v2-blue.svg)](https://github.com/cristianst85/WMR100.NET/blob/master/LICENSE)

## Fully Supported & Tested Devices

* Oregon Scientific RMS600

## Prerequisites

* Linux with Mono version 6.8.0 (or newer), but should also work with older versions of Mono as low as 5.4.1 without any issues.
* (USB Library) `libusb-1_0-0` package.
* Oregon Scientific weather station that uses the WMR100 protocol.

## Linux Setup

If you have installed the `libusb-1_0-0` package and you still have an error about loading library, it may be needed to make a symlink to allow runtime load the library.

First, find the location of the library. For example, `sudo find / -name "libusb-1.0*.so*"` can give you:

```
/usr/lib64/libusb-1.0.so.0.3.0
/usr/lib64/libusb-1.0.so.0
```

Then go to the directory, and make the symlink. It should match the library name, with extension (.so), but without the version:

```
cd /usr/lib64
sudo ln -s libusb-1.0.so.0 libusb-1.0.so
```

## Testing

This library was tested on openSUSE Leap 15.5 with Mono version 6.8.0.105 and an Oregon Scientific RMS600 Advanced Weather Station.

## Repository

The main repository is hosted on [GitHub](https://github.com/cristianst85/WMR100.NET).

## Changelog

See [CHANGELOG](https://github.com/cristianst85/WMR100.NET/blob/master/CHANGELOG.md) file for details.

## License

The source code in this repository is released under the GNU GPLv2 or later license. See the [bundled LICENSE](https://github.com/cristianst85/WMR100.NET/blob/master/LICENSE) file for details.

## Resources

The following resources were used for figuring out the WMR100 protocol:

 * https://github.com/barnybug-archive/wmr100
 * https://github.com/ejeklint/WLoggerDaemon/blob/master/Station_protocol.md
 * http://www.cs.stir.ac.uk/~kjt/software/comms/wmr180.html (link is dead as of April 14, 2024, but a snapshot from the Internet Archive can be accessed [here](https://web.archive.org/web/20200107095622/https://www.cs.stir.ac.uk/~kjt/software/comms/wmr180.html)).
