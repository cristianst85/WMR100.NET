using WMR100.NET.Data;

namespace WMR100.NET.SensorData
{
    public abstract class Wmr100SensorData : Wmr100Data, IWmr100SensorData
    {
        public int SensorId { get; protected set; }
        public BatteryLevelStatus BatteryLevelStatus { get; protected set; }
    }
}
