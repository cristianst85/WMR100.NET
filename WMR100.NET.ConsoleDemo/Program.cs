using LibUsbDotNet;
using Newtonsoft.Json;
using System;
using System.IO;
using WMR100.NET.Helpers;

namespace WMR100.NET.ConsoleDemo
{
    public class Program
    {
        public static void Main()
        {
            var t = Type.GetType("Mono.Runtime") ?? throw new NotImplementedException();
            InitMono();
        }

        private static void InitMono()
        {
            // Subscribe to this event to see the errors from low-level API.
            UsbDevice.UsbErrorEvent += UsbDevice_UsbErrorEvent;
            WmrUsbDevice.Log += (message) => LogToConsole(message);

            using (var wmrUsbDevice = WmrUsbDevice.Create())
            {
                var wmr100Device = new Wmr100Device(wmrUsbDevice, new Wmr100DataFrameAssembler());

                wmr100Device.Init();

                wmr100Device.DataReceived += Wmr100Device_DataReceived;
                wmr100Device.DataError += Wmr100Device_DataError;
                wmr100Device.Error += Wmr100Device_Error;

                LogToConsole("Receiving data...");
                wmr100Device.ReceiveData();
            }

            LogToConsole("Application was closed.");
            Environment.Exit(0);
        }

        private static void UsbDevice_UsbErrorEvent(object sender, UsbError e)
        {
            LogToConsole($"USB device error: {e.ErrorCode} {e.Description} {e.Win32ErrorNumber} {e.Win32ErrorString}");
        }

        private static void Wmr100Device_Error(object sender, ErrorEventArgs e)
        {
            LogToConsole($"Device error. Exception: {e.GetException()}");

            if (sender is Wmr100Device wmr100Device)
            {
                LogToConsole("Stopping device...");
                wmr100Device.Stop();
            }
        }

        private static void Wmr100Device_DataError(object sender, DataErrorEventArgs e)
        {
            var hexFrameData = ByteArrayUtils.ByteArrayToString(e.FrameData);

            if (!e.ChecksumValid)
            {
                LogToConsole($"Bad data frame: {hexFrameData} (invalid checksum)");
            }
            else if (!e.LengthValid)
            {
                LogToConsole($"Bad data frame: {hexFrameData} (invalid length)");
            }
        }

        private static void Wmr100Device_DataReceived(object sender, DataReceivedEventArgs e)
        {
            LogToConsole($"Data received: {ByteArrayUtils.ByteArrayToString(e.PacketData)}");

            if (Wmr100Data.TryDecode(e.PacketData, out Wmr100Data wmr100Data))
            {
                LogToConsole($"Decoded data: {JsonConvert.SerializeObject(wmr100Data)}");
            }
            else
            {
                LogToConsole($"Cannot decode data.");
            }
        }

        private static void LogToConsole(string message)
        {
            Console.WriteLine(message);
        }
    }
}
