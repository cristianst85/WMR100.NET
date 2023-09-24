using System;

namespace WMR100.NET
{
    public class DataReceivedEventArgs : EventArgs
    {
        public byte[] PacketData { get; private set; }

        public Wmr100Data Data { get; private set; }

        public DataReceivedEventArgs(byte[] packetData, Wmr100Data data)
        {
            this.PacketData = packetData;
            this.Data = data;
        }
    }
}
