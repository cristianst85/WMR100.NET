using LibUsbDotNet;
using LibUsbDotNet.LudnMonoLibUsb;
using LibUsbDotNet.Main;
using MonoLibUsb;
using System;

namespace WMR100.NET
{
    public class WmrUsbDevice : IDisposable, IWmrUsbDevice
    {
        public static Action<string> Log;

        private UsbDevice usbDevice;
        private UsbEndpointReader usbEndpointReader;

        public static WmrUsbDevice Create()
        {
            UsbDevice usbDevice = null;

            try
            {
                var descriptor = Wmr100Device.Descriptor;
                var usbDeviceFinder = new UsbDeviceFinder(descriptor.Vendor.Id, descriptor.Product.Id);

                InternalLog("Opening USB device...");
                usbDevice = UsbDevice.OpenUsbDevice(usbDeviceFinder);

                if (usbDevice is MonoUsbDevice monoUsbDevice)
                {
                    InternalLog("Opening USB device handle...");
                    var monoUsbDeviceHandle = monoUsbDevice.Profile.OpenDeviceHandle();

                    InternalLog("Enabling auto detach kernel driver...");
                    MonoUsbApi.SetAutoDetachKernelDriver(monoUsbDeviceHandle, 1);

                    InternalLog("Determine if kernel driver is active on interface #0...");
                    int kernelDriverStatus = MonoUsbApi.KernelDriverActive(monoUsbDeviceHandle, 0);

                    if (kernelDriverStatus == 1)
                    {
                        InternalLog("Kernel driver is active on interface #0. Detaching...");
                        int errorCode = MonoUsbApi.DetachKernelDriver(monoUsbDeviceHandle, 0);

                        if (errorCode != 0)
                        {
                            throw new Exception($"Failed to detach kernel driver (error code: {errorCode}).");
                        }
                    }
                    else if (kernelDriverStatus < 0)
                    {
                        throw new Exception($"Failed to get kernel driver status (error code: {kernelDriverStatus}).");
                    }
                }

                if (usbDevice == null)
                {
                    throw new Exception("USB device was not found.");
                }
                else
                {
                    InternalLog($"Found device with ID {usbDevice.Info.Descriptor.VendorID:x4}:{usbDevice.Info.Descriptor.ProductID:x4} {descriptor.GetName()}.");
                    IUsbDevice wholeUsbDevice = usbDevice as IUsbDevice;

                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Select configuration #1.
                        bool success = wholeUsbDevice.SetConfiguration(1);
                        if (!success)
                        {
                            throw new Exception("Failed to set USB device configuration #1.");
                        }

                        // Claim interface #0.
                        success = wholeUsbDevice.ClaimInterface(0);
                        if (!success)
                        {
                            throw new Exception("Failed to claim USB device on interface #0.");
                        }
                    }

                    InternalLog("WMR USB device was successfully instantiated.");
                    return new WmrUsbDevice(usbDevice);
                }
            }
            catch
            {
                if (usbDevice != null)
                {
                    if (usbDevice.IsOpen)
                    {
                        if (usbDevice is IUsbDevice wholeUsbDevice)
                        {
                            // Release interface #0.
                            bool success = wholeUsbDevice.ReleaseInterface(0);
                            InternalLog($"{(success ? "Successfully released" : "Failed to release")} USB device interface #0.");
                        }

                        usbDevice.Close();
                    }
                }

                UsbDevice.Exit();

                InternalLog("De-initialized USB driver.");

                MonoUsbEventHandler.Stop(false);
                MonoUsbEventHandler.Exit();

                InternalLog("Stopped USB event handler thread.");
                throw;
            }
        }
        
        private WmrUsbDevice(UsbDevice UsbDevice)
        {
            this.usbDevice = UsbDevice;
            this.usbEndpointReader = usbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
        }

        public byte[] Read()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(usbEndpointReader));
            }

            byte[] buffer = new byte[8];

            ErrorCode errorCode = usbEndpointReader.Read(buffer, 30000, out int bytesRead);

            if (bytesRead == 0)
            {
                throw new Exception($"No data read from USB device (error code: {errorCode}).");
            }

            if (errorCode != ErrorCode.None)
            {
                throw new Exception($"Error reading data from USB device (error code: {errorCode}).");
            }

            return buffer;
        }

        public void Write(byte[] buffer)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(usbDevice));
            }

            var usbSetupPacket = WmrUsbSetupPacket.Create();
            bool success = usbDevice.ControlTransfer(ref usbSetupPacket, buffer, buffer.Length, out int bytesTransferred);

            if (!success)
            {
                throw new Exception("Writing control data to USB device failed.");
            }
        }

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (usbEndpointReader != null)
                    {
                        if (!usbEndpointReader.IsDisposed)
                        {
                            usbEndpointReader.Dispose();
                        }
                    }

                    if (usbDevice != null)
                    {
                        if (usbDevice.IsOpen)
                        {
                            var wholeUsbDevice = usbDevice as IUsbDevice;

                            if (wholeUsbDevice != null)
                            {
                                // Release interface #0.
                                bool success = wholeUsbDevice.ReleaseInterface(0);

                                InternalLog($"{(success ? "Successfully released" : "Failed to release")} USB device interface #0.");
                            }

                            usbDevice.Close();
                        }
                    }

                    UsbDevice.Exit();

                    InternalLog("De-initialized USB driver.");

                    MonoUsbEventHandler.Stop(false);
                    MonoUsbEventHandler.Exit();

                    InternalLog("Stopped USB event handler thread.");

                    usbDevice = null;
                    usbEndpointReader = null;
                    disposed = true;
                }
            }
        }

        private static void InternalLog(string message)
        {
            Log?.Invoke(message);
        }
    }
}
