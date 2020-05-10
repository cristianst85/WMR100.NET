using System;
using System.IO;
using System.Threading;
using WMR100.NET.Data;
using WMR100.NET.Helpers;

namespace WMR100.NET
{

    public delegate void DataRecievedEventHandler(object sender, DataRecievedEventArgs e);
    public delegate void DataDecodeErrorEventHandler(object sender, DataDecodeErrorEventArgs e);
    public delegate void DataErrorEventHandler(object sender, DataErrorEventArgs e);

    public class Wmr100Device : IWmr100Device
    {

        public static readonly VidPidDescriptor Descriptor = VidPidDescriptor.Wmr100;

        private static readonly byte[] initRequest = { 0x20, 0x00, 0x08, 0x01, 0x00, 0x00, 0x00, 0x00 };
        private static readonly byte[] sendDataRequest = { 0x01, 0xd0, 0x08, 0x01, 0x00, 0x00, 0x00, 0x00 };

        private volatile bool stopped = false;

        private IWmrUsbDevice wmrUsbDevice;
        private Wmr100DataFrameAssembler dataFrameAssembler;

        public event DataRecievedEventHandler DataRecieved;
        public event DataDecodeErrorEventHandler DataDecodeError;
        public event DataErrorEventHandler DataError;
        public event ErrorEventHandler Error;

        public Wmr100Device(IWmrUsbDevice wmrUsbDevice)
        {
            this.wmrUsbDevice = wmrUsbDevice;
            this.dataFrameAssembler = new Wmr100DataFrameAssembler();
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
                    var dataFrames = dataFrameAssembler.Assemble(wmrUsbDevice.Read());
                    foreach (var dataFrame in dataFrames)
                    {
                        // Verify frame checksum.
                        bool checksumValid = dataFrame.IsChecksumValid();

                        // Verify frame length.
                        bool lengthValid = dataFrame.IsLengthValid();

                        if (checksumValid && lengthValid)
                        {
                            Wmr100Data wmr100Data = null;

                            bool success = Wmr100Data.TryDecode(dataFrame.GetPacketData(), out wmr100Data);

                            if (success)
                            {
                                DataRecievedEventHandler handler = DataRecieved;
                                if (handler != null)
                                {
                                    handler(this, new DataRecievedEventArgs(dataFrame.GetPacketData(), wmr100Data));
                                }
                            }
                            else
                            {
                                DataDecodeErrorEventHandler handler = DataDecodeError;
                                if (handler != null)
                                {
                                    handler(this, new DataDecodeErrorEventArgs(dataFrame.GetPacketData()));
                                }
                            }
                        }
                        else
                        {
                            DataErrorEventHandler handler = DataError;
                            if (handler != null)
                            {
                                handler(this, new DataErrorEventArgs(dataFrame.Data, checksumValid, lengthValid));
                            }
                        }
                    }

                    if (dataFrames.Count > 0)
                    {
                        wmrUsbDevice.Write(sendDataRequest);
                    }
                }
                catch (Exception ex)
                {
                    ErrorEventHandler handler = Error;
                    if (handler != null)
                    {
                        handler(this, new ErrorEventArgs(ex));
                    }
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
