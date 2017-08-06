namespace WMR100.NET
{
    public class DataDecodeErrorEventArgs
    {
        public byte[] PacketData { get; private set; }
        public DataDecodeErrorEventArgs(byte[] packetData)
        {
            this.PacketData = packetData;
        }
    }
}