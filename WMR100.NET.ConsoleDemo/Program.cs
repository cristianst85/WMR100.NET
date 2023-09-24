using LibUsbDotNet;
using System;
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

            using (var wmrUsbDevice = WmrUsbDevice.Create())
            {
                var wmr100Device = new Wmr100Device(wmrUsbDevice, new Wmr100DataFrameAssembler());

                wmr100Device.Init();

                wmr100Device.DataReceived += Wmr100Device_DataReceived;
                wmr100Device.DataError += Wmr100Device_DataError;
                wmr100Device.Error += Wmr100Device_Error;

                wmr100Device.ReceiveData();
            }
        }

        private static void UsbDevice_UsbErrorEvent(object sender, UsbError e)
        {
            Console.WriteLine(string.Format("USB device error: {0} {1} {2} {3}", e.ErrorCode, e.Description, e.Win32ErrorNumber, e.Win32ErrorString));
        }

        private static void Wmr100Device_Error(object sender, System.IO.ErrorEventArgs e)
        {
            Console.WriteLine(string.Format("Device error. Exception: {0}", e.GetException()));

            if (sender is Wmr100Device wmr100Device)
            {
                Console.WriteLine("Stopping device...");
                wmr100Device.Stop();
            }
        }

        private static void Wmr100Device_DataError(object sender, DataErrorEventArgs e)
        {
            var hexFrameData = ByteArrayUtils.ByteArrayToString(e.FrameData);

            if (!e.ChecksumValid)
            {
                Console.WriteLine(string.Format("Bad data frame: {0} (invalid checksum)", hexFrameData));
            }
            else if (!e.LengthValid)
            {
                Console.WriteLine(string.Format("Bad data frame: {0} (invalid length)", hexFrameData));
            }
        }

        private static void Wmr100Device_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(string.Format("Data received: {0}", ByteArrayUtils.ByteArrayToString(e.PacketData)));

            if (Wmr100Data.TryDecode(e.PacketData, out Wmr100Data wmr100Data))
            {
                Console.WriteLine(string.Format("Decoded data: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(wmr100Data)));
            }
            else
            {
                Console.WriteLine("Cannot decode data!");
            }
        }
    }
}
