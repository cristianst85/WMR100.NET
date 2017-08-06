using System.IO;

namespace WMR100.NET
{
    public interface IWmr100Device
    {
        event DataDecodeErrorEventHandler DataDecodeError;
        event DataErrorEventHandler DataError;
        event DataRecievedEventHandler DataRecieved;
        event ErrorEventHandler Error;

        void Init();

        void ReceiveData();

        void Stop();
    }
}