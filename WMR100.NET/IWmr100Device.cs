using System.IO;

namespace WMR100.NET
{
    public interface IWmr100Device
    {
        event DataDecodeErrorEventHandler DataDecodeError;

        event DataFrameErrorEventHandler DataFrameError;

        event DataReceivedEventHandler DataReceived;

        event ErrorEventHandler Error;

        void Init();

        void ReceiveData();

        void Stop();
    }
}