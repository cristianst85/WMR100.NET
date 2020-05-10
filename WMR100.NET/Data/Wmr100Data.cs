using System;
using WMR100.NET.SensorData;

namespace WMR100.NET.Data
{
    /// <summary>
    /// The following references were used for figuring out the WMR100 protocol:
    /// 
    /// From Barnaby Gray:
    ///     https://github.com/barnybug-archive/wmr100
    /// 
    /// From Per Ejeklint:
    ///     https://github.com/ejeklint/WLoggerDaemon/blob/master/Station_protocol.md
    /// </summary>
    public abstract class Wmr100Data : IWmr100Data
    {
        public virtual Wmr100DataType Type { get; private set; }
        public static bool TryDecode(byte[] packetData, out Wmr100Data wmr100Data)
        {
            wmr100Data = null;
            byte dataType = packetData[1];

            if (dataType == (byte)Wmr100DataType.Clock)
            {
                // Flags from byte 0.
                int flags = (byte)(packetData[0]) >> 4;
                bool isPowered = Convert.ToBoolean((flags & 0x8) >> 3);
                bool hasLowBattery = Convert.ToBoolean((flags & 0x4) >> 2);
                bool isRFSyncEnabled = Convert.ToBoolean((flags & 0x2) >> 1);
                bool isRFLevelStrong = Convert.ToBoolean(flags & 0x1);

                int minute = packetData[4];
                int hour = packetData[5];
                int day = packetData[6];
                int month = packetData[7];
                int year = 2000 + packetData[8];
                int timeZoneOffset = packetData[9];

                if (timeZoneOffset > 128)
                {
                    timeZoneOffset = timeZoneOffset - 256;
                }

                DateTime clock = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Local);
                ClockData clockData = new ClockData(clock, timeZoneOffset, isPowered, hasLowBattery, isRFSyncEnabled, isRFLevelStrong);

                wmr100Data = clockData;
                return true;
            }
            else if (dataType == (byte)Wmr100DataType.TemperatureHumidity)
            {
                bool hasLowBattery = Convert.ToBoolean((packetData[0] & 0x40) >> 6);

                int temperatureTrend = (packetData[0] & 0x30) >> 4;
                int sensorId = packetData[2] & 0x0f;

                int comfortLevel = packetData[2] >> 6;
                int humidityTrend = (packetData[2] >> 4) & 0x03;

                decimal temperature = (packetData[3] + ((packetData[4] & 0x0f) << 8)) / 10.0m;

                if ((packetData[4] >> 4) == 0x8)
                {
                    temperature = -temperature;
                }

                decimal humidity = packetData[5];

                // DewPoint on Channel 0 is not provided (i.e.: always 0) for Oregon Scientific RMS600.
                decimal dewPoint = (packetData[6] + ((packetData[7] & 0x0f) << 8)) / 10.0m;

                if ((packetData[7] >> 4) == 0x8)
                {
                    dewPoint = -dewPoint;
                }

                bool isHeatIndexValid = !Convert.ToBoolean((packetData[9] & 0x20) >> 5);

                decimal? heatIndex = null;
                if (isHeatIndexValid)
                {
                	// HeatIndex is in Fahrenheit.
                    heatIndex = (packetData[8] + ((packetData[9] & 0x0f) << 8)) / 10.0m;
                }

                TemperatureHumidityData temperatureHumidityData = new TemperatureHumidityData(sensorId, temperature, temperatureTrend, humidity, humidityTrend,  dewPoint, comfortLevel, heatIndex, hasLowBattery);

                wmr100Data = temperatureHumidityData;
                return true;
            }
            else
            {
                return false; // Not implemented yet :)
            }
        }
    }
}
