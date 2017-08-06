using LibUsbDotNet;
using LibUsbDotNet.LudnMonoLibUsb;
using LibUsbDotNet.Main;
using MonoLibUsb;
using System;

namespace WMR100.NET
{
    public class WmrUsbDevice : IDisposable, IWmrUsbDevice
    {
        private bool _disposed;

        private UsbDevice usbDevice;
        private UsbEndpointReader usbEndpointReader;

        public static WmrUsbDevice Create()
        {
            UsbDevice usbDevice = null;
            try
            {
                var descriptor = Wmr100Device.Descriptor;
                var usbDeviceFinder = new UsbDeviceFinder(descriptor.Vendor.Id, descriptor.Product.Id);
                usbDevice = UsbDevice.OpenUsbDevice(usbDeviceFinder);
                MonoUsbDevice monoUsbDevice = usbDevice as MonoUsbDevice;
                if (monoUsbDevice != null)
                {
                    var monoUsbDeviceHandle = monoUsbDevice.Profile.OpenDeviceHandle();
                    Console.WriteLine("Setting auto detach kernel driver true...");
                    MonoUsbApi.SetAutoDetachKernelDriver(monoUsbDeviceHandle, 1);
                    int kernelDriverStatus = MonoUsbApi.KernelDriverActive(monoUsbDeviceHandle, 0);
                    if (kernelDriverStatus == 1)
                    {
                        Console.WriteLine("Kernel driver is active. Detaching...");
                        int errorCode = MonoUsbApi.DetachKernelDriver(monoUsbDeviceHandle, 0);
                        if (errorCode != 0)
                        {
                            throw new Exception(string.Format("Failed to detach kernel driver (error code: {0}).", errorCode));
                        }
                    }
                    else if (kernelDriverStatus < 0)
                    {
                        throw new Exception(string.Format("Failed to get kernel driver status (error code: {0}).", kernelDriverStatus));
                    }
                }
                if (usbDevice == null)
                {
                    throw new Exception("USB device was not found.");
                }
                else
                {
                    Console.WriteLine("Found device with ID {0:x4}:{1:x4} {2}", usbDevice.Info.Descriptor.VendorID, usbDevice.Info.Descriptor.ProductID, descriptor.GetName());
                    IUsbDevice wholeUsbDevice = usbDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Select configuration #1
                        bool success = wholeUsbDevice.SetConfiguration(1);
                        if (!success)
                        {
                            throw new Exception("Failed to set USB device configuration.");
                        }
                        // Claim interface #0.
                        success = wholeUsbDevice.ClaimInterface(0);
                        if (!success)
                        {
                            throw new Exception("Failed to claim USB device interface.");
                        }
                    }
                    return new WmrUsbDevice(usbDevice);
                }
            }
            catch
            {
                if (usbDevice != null)
                {
                    if (usbDevice.IsOpen)
                    {
                        IUsbDevice wholeUsbDevice = usbDevice as IUsbDevice;
                        if (wholeUsbDevice != null)
                        {
                            // Release interface #0.
                            bool success = wholeUsbDevice.ReleaseInterface(0);
                            Console.WriteLine(string.Format("{0} USB device interface.", success ? "Released" : "Failed to release"));
                        }
                        usbDevice.Close();
                    }
                    usbDevice = null;
                }
                UsbDevice.Exit();
                Console.WriteLine("De-initialized USB driver.");
                MonoUsbEventHandler.Stop(false);
                MonoUsbEventHandler.Exit();
                Console.WriteLine("Stopped USB event handler thread.");
                throw;
            }
        }
        
        public WmrUsbDevice(UsbDevice UsbDevice)
        {
            this.usbDevice = UsbDevice;
            this.usbEndpointReader = usbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
        }

        public byte[] Read()
        {
            byte[] buffer = new byte[8];

            int bytesRead = 0;
            ErrorCode errorCode = usbEndpointReader.Read(buffer, 30000, out bytesRead);

            if (bytesRead == 0)
            {
                throw new Exception(string.Format("No data read from USB device (error code: {0}).", errorCode));
            }
            if (errorCode != ErrorCode.None)
            {
                throw new Exception(string.Format("Error reading data from USB device (error code: {0}).", errorCode));
            }
            return buffer;
        }

        public void Write(byte[] buffer)
        {
            int bytesTransferred = 0;
            UsbSetupPacket usbSetupPacket = WmrUsbSetupPacket.Create();
            bool success = usbDevice.ControlTransfer(ref usbSetupPacket, buffer, buffer.Length, out bytesTransferred);
            if (!success)
            {
                throw new Exception("Writing control data to USB device failed.");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
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
                            IUsbDevice wholeUsbDevice = usbDevice as IUsbDevice;
                            if (wholeUsbDevice != null)
                            {
                                // Release interface #0.
                                bool success = wholeUsbDevice.ReleaseInterface(0);
                                Console.WriteLine(string.Format("{0} USB device interface.", success ? "Released" : "Failed to release"));
                            }
                            usbDevice.Close();
                        }
                    }

                    UsbDevice.Exit();
                    Console.WriteLine("De-initialized USB driver.");
                    MonoUsbEventHandler.Stop(false);
                    MonoUsbEventHandler.Exit();
                    Console.WriteLine("Stopped USB event handler thread.");
                }

                // Indicate that the instances had been disposed.
                usbDevice = null;
                usbEndpointReader = null;
                _disposed = true;
            }
        }
    }
}
