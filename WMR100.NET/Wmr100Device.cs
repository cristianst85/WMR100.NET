using System;
using System.IO;
using System.Threading;

namespace WMR100.NET
{
    public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);

    public delegate void DataDecodeErrorEventHandler(object sender, DataDecodeErrorEventArgs e);

    public delegate void DataErrorEventHandler(object sender, DataFrameErrorEventArgs e);

    public class Wmr100Device : IWmr100Device, IDisposable
    {
        public static Action<string> Log;

        public static readonly VidPidDescriptor Descriptor = VidPidDescriptor.Wmr100;

        private static readonly byte[] initRequest = { 0x20, 0x00, 0x08, 0x01, 0x00, 0x00, 0x00, 0x00 };
        private static readonly byte[] sendDataRequest = { 0x01, 0xd0, 0x08, 0x01, 0x00, 0x00, 0x00, 0x00 };

        private volatile bool stopped = false;

        private readonly WmrUsbDevice wmrUsbDevice;
        private readonly Wmr100DataFrameAssembler wmrDataFrameAssembler;

        public event DataReceivedEventHandler DataReceived;
        public event DataDecodeErrorEventHandler DataDecodeError;
        public event DataErrorEventHandler DataError;
        public event ErrorEventHandler Error;

        public static Wmr100Device Create()
        {
            if (Log != null)
            {
                WmrUsbDevice.Log += (message) => Log(message);
                Wmr100DataFrameAssembler.Log += (message) => Log(message);
            }

            return new Wmr100Device(WmrUsbDevice.Create(), new Wmr100DataFrameAssembler());
        }

        private Wmr100Device(WmrUsbDevice wmrUsbDevice, Wmr100DataFrameAssembler wmrDataFrameAssembler)
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
                        // Verify the frame length first.
                        if (!dataFrame.IsLengthValid())
                        {
                            DataError?.Invoke(this, new DataFrameErrorEventArgs(dataFrame.Data, DataFrameErrorType.InvalidDataFrameLength));
                            continue;
                        }

                        // Verify frame checksum.
                        if (!dataFrame.IsChecksumValid())
                        {
                            DataError?.Invoke(this, new DataFrameErrorEventArgs(dataFrame.Data, DataFrameErrorType.InvalidDataFrameChecksum));
                            continue;
                        }

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

        public virtual void Stop()
        {
            stopped = true;
        }

        public void Dispose()
        {
            Stop();
            wmrUsbDevice?.Dispose();
        }
    }
}
