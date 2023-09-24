using System;
using System.IO;
using System.Threading;
using WMR100.NET.Helpers;

namespace WMR100.NET
{
    public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);

    public delegate void DataDecodeErrorEventHandler(object sender, DataDecodeErrorEventArgs e);

    public delegate void DataErrorEventHandler(object sender, DataErrorEventArgs e);

    public class Wmr100Device : IWmr100Device
    {
        public static readonly VidPidDescriptor Descriptor = VidPidDescriptor.Wmr100;

        private static readonly byte[] initRequest = { 0x20, 0x00, 0x08, 0x01, 0x00, 0x00, 0x00, 0x00 };
        private static readonly byte[] sendDataRequest = { 0x01, 0xd0, 0x08, 0x01, 0x00, 0x00, 0x00, 0x00 };

        private volatile bool stopped = false;

        private readonly IWmrUsbDevice wmrUsbDevice;
        private readonly IWmr100DataFrameAssembler wmrDataFrameAssembler;

        public event DataReceivedEventHandler DataReceived;
        public event DataDecodeErrorEventHandler DataDecodeError;
        public event DataErrorEventHandler DataError;
        public event ErrorEventHandler Error;

        public Wmr100Device(IWmrUsbDevice wmrUsbDevice, IWmr100DataFrameAssembler wmrDataFrameAssembler)
        {
            this.wmrUsbDevice = wmrUsbDevice;
            this.wmrDataFrameAssembler = wmrDataFrameAssembler;
        }

        public virtual void Init()
        {
            wmrUsbDevice.Write(initRequest);
            wmrUsbDevice.Write(sendDataRequest);
        }

        public virtual void ReceiveData()
        {
            while (!stopped)
            {
                try
                {
                    var dataFrames = wmrDataFrameAssembler.Assemble(wmrUsbDevice.Read());

                    foreach (var dataFrame in dataFrames)
                    {
                        // Verify frame checksum.
                        bool checksumValid = dataFrame.IsChecksumValid();

                        // Verify frame length.
                        bool lengthValid = dataFrame.IsLengthValid();

                        if (checksumValid && lengthValid)
                        {
                            bool success = Wmr100Data.TryDecode(dataFrame.GetPacketData(), out Wmr100Data wmr100Data);

                            if (success)
                            {
                                DataReceived?.Invoke(this, new DataReceivedEventArgs(dataFrame.GetPacketData(), wmr100Data));
                            }
                            else
                            {
                                DataDecodeError?.Invoke(this, new DataDecodeErrorEventArgs(dataFrame.GetPacketData()));
                            }
                        }
                        else
                        {
                            DataError?.Invoke(this, new DataErrorEventArgs(dataFrame.Data, checksumValid, lengthValid));
                        }
                    }

                    if (dataFrames.Count > 0)
                    {
                        wmrUsbDevice.Write(sendDataRequest);
                    }
                }
                catch (Exception ex)
                {
                    Error?.Invoke(this, new ErrorEventArgs(ex));

                    Thread.Sleep(1000);
                }
            }
        }

        public void Stop()
        {
            this.stopped = true;
        }
    }
}
