namespace WMR100.NET
{
    public interface IWmrUsbDevice {

        byte[] Read();

        void Write(byte[] buffer);
    }
}