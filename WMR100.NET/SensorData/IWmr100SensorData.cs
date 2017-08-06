using WMR100.NET.Data;

namespace WMR100.NET.SensorData
{
    public interface IWmr100SensorData : IWmr100Data
    {
        int SensorId { get; }
    }
}
