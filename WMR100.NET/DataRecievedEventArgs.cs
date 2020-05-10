using System;
using WMR100.NET.Data;

namespace WMR100.NET
{
    public class DataRecievedEventArgs : EventArgs
    {
        public byte[] PacketData { get; private set; }

        public Wmr100Data Data { get; private set; }

        public DataRecievedEventArgs(byte[] packetData, Wmr100Data data)
        {
            this.PacketData = packetData;
            this.Data = data;
        }
    }
}
