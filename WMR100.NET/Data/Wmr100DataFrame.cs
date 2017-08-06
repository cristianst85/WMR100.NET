using System;

namespace WMR100.NET.Data
{
    /// <summary>
    /// The frame structure is as follows,
    /// the length being implied by the response code: 
    /// <para>
    /// FD FD ST RC D1 ... DN C1 C2
    /// </para>
    /// <para>    
    /// FD (frame delimiter)
    ///      FF
    ///      Two of these delimiters start each frame.
    /// ST (status)
    ///      00 - FF
    ///      Identifies the battery level, for example.
    /// RC (response code)
    ///      00 - FF
    ///      Identifies the type of sensor.
    /// D1 ... DN (data)
    ///      00 - FF
    ///      Sensor data. 
    /// C1 C2 (checksum bytes)
    ///      Add all frame bytes as unsigned integers, excluding the delimiter and checksum bytes. 
    ///      For a valid frame, this sum must equal C1+256 * C2. Since this is quite a weak checksum, 
    ///      it is also advisable to check that a frame has the length implied by the response code.
    /// </para>
    /// <para>The actual frame data depends on the type of sensor.</para>
    /// 
    /// <para>
    /// Source: http://www.cs.stir.ac.uk/~kjt/software/comms/wmr180.html
    /// </para>
    /// </summary>
    public class Wmr100DataFrame
    {

        public static readonly byte Delimiter = 0xff;

        public byte[] Data { get; private set; }

        public Wmr100DataFrame(byte[] data)
        {
            this.Data = data;
        }

        public bool IsChecksumValid()
        {
            int computedChecksum = 0;
            for (int i = 2; i < Data.Length - 2; i++)
            {
                computedChecksum += (int)(Data[i]);
            }
            int frameChecksum = (((int)Data[Data.Length - 1]) << 8) + ((int)Data[Data.Length - 2]);
            return frameChecksum == computedChecksum;
        }

        public bool IsLengthValid()
        {
            var packetLength = Data.Length - 2; // Length without the two frame delimiters.
            if (packetLength < 0)
            {
                return false;
            }
            else
            {
                var dataType = (byte)Data[3];
                if (dataType == (byte)Wmr100DataType.Rain && packetLength == 17)
                {
                    return true;
                }
                else if (dataType == (byte)Wmr100DataType.TemperatureHumidity && packetLength == 12)
                {
                    return true;
                }
                else if (dataType == (byte)Wmr100DataType.TemperatureWater && packetLength == 7)
                {
                    return true;
                }
                else if (dataType == (byte)Wmr100DataType.Pressure && packetLength == 8)
                {
                    return true;
                }
                else if (dataType == (byte)Wmr100DataType.UV && packetLength == 5)
                {
                    return true;
                }
                else if (dataType == (byte)Wmr100DataType.Wind && packetLength == 11)
                {
                    return true;
                }
                else if (dataType == (byte)Wmr100DataType.Clock && packetLength == 12)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public byte[] GetPacketData()
        {
            var packetData = new byte[Data.Length - 4];
            Array.Copy(Data, 2, packetData, 0, packetData.Length);
            return packetData;
        }
    }
}
