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
            _ = Type.GetType("Mono.Runtime") ?? throw new NotImplementedException();
            InitMono();
        }

        private static void InitMono()
        {
            // Subscribe to this event to see the errors from low-level API.
            UsbDevice.UsbErrorEvent += UsbDevice_UsbErrorEvent;
            Wmr100Device.Log += (message) => LogToConsole(message);

            using (var wmr100Device = Wmr100Device.Create())
            {
                wmr100Device.Init();

                wmr100Device.DataReceived += Wmr100Device_DataReceived;
                wmr100Device.DataDecodeError += Wmr100Device_DataDecodeError;
                wmr100Device.DataFrameError += Wmr100Device_DataFrameError;
                wmr100Device.Error += Wmr100Device_Error;

                LogToConsole("Receiving data...");
                wmr100Device.ReceiveData();
            }

            LogToConsole("Application was closed.");
            Environment.Exit(0);
        }

        private static void Wmr100Device_DataDecodeError(object sender, DataDecodeErrorEventArgs e)
        {
            LogToConsole($"Cannot decode packet data: {ByteArrayUtils.ByteArrayToString(e.PacketData)}");
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
                Stop(wmr100Device);
            }
        }

        private static void Wmr100Device_DataFrameError(object sender, DataFrameErrorEventArgs e)
        {
            var hexFrameData = ByteArrayUtils.ByteArrayToString(e.FrameData);

            if (e.ErrorType == DataFrameErrorType.InvalidDataFrameLength)
            {
                LogToConsole($"Bad data frame: {hexFrameData} (invalid length)");
            }
            else if (e.ErrorType == DataFrameErrorType.InvalidDataFrameChecksum)
            {
                LogToConsole($"Bad data frame: {hexFrameData} (invalid checksum)");
            }
        }

        private static void Wmr100Device_DataReceived(object sender, DataReceivedEventArgs e)
        {
            LogToConsole($"Data packet received: {ByteArrayUtils.ByteArrayToString(e.PacketData)}");
            LogToConsole($"Decoded data: {JsonConvert.SerializeObject(e.Data)}");
        }

        private static void Stop(Wmr100Device wmr100Device)
        {
            wmr100Device.Stop();
            wmr100Device.DataReceived -= Wmr100Device_DataReceived;
            wmr100Device.DataDecodeError -= Wmr100Device_DataDecodeError;
            wmr100Device.DataFrameError -= Wmr100Device_DataFrameError;
            wmr100Device.Error -= Wmr100Device_Error;

            UsbDevice.UsbErrorEvent -= UsbDevice_UsbErrorEvent;
        }

        private static void LogToConsole(string message)
        {
            Console.WriteLine(message);
        }
    }
}
