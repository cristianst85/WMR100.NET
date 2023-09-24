using System.IO;

namespace WMR100.NET
{
    public interface IWmr100Device
    {
        event DataDecodeErrorEventHandler DataDecodeError;

        event DataErrorEventHandler DataError;

        event DataReceivedEventHandler DataReceived;

        event ErrorEventHandler Error;

        void Init();

        void ReceiveData();

        void Stop();
    }
}