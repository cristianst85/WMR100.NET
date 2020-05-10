using System;
using WMR100.NET.Data;

namespace WMR100.NET.SensorData
{
    public class ClockData : Wmr100Data, IEquatable<ClockData>
    {
        public override Wmr100DataType Type
        {
            get
            {
                return Wmr100DataType.Clock;
            }
        }
        public DateTime Clock { get; private set; }
        public int TimeZoneOffset { get; private set; }
        public PowerConnectorStatus PowerConnectorStatus { get; private set; }
        public BatteryLevelStatus BatteryLevelStatus { get; private set; }
        public RFClockSyncStatus RFClockSyncStatus { get; private set; }
        public RFClockSignalLevelStatus RFClockSignalLevelStatus { get; private set; }

        public ClockData(DateTime clock, int timeZoneOffset, PowerConnectorStatus powerConnectorStatus, BatteryLevelStatus batteryLevelStatus, RFClockSyncStatus rfClockSyncStatus, RFClockSignalLevelStatus rfClockSignalLevelStatus)
        {
            this.Clock = clock;
            this.TimeZoneOffset = timeZoneOffset;
            this.PowerConnectorStatus = powerConnectorStatus;
            this.BatteryLevelStatus = batteryLevelStatus;
            this.RFClockSyncStatus = rfClockSyncStatus;
            this.RFClockSignalLevelStatus = rfClockSignalLevelStatus;
        }

        public ClockData(DateTime clock, int timeZoneOffset, bool isPowered, bool hasLowBattery, bool isRFSyncEnabled, bool isRFLevelStrong)
        {
            this.Clock = clock;
            this.TimeZoneOffset = timeZoneOffset;
            this.PowerConnectorStatus = isPowered ? PowerConnectorStatus.Connected : PowerConnectorStatus.NotConnected;
            this.BatteryLevelStatus = hasLowBattery ? BatteryLevelStatus.Low : BatteryLevelStatus.Normal;
            this.RFClockSyncStatus = isRFSyncEnabled ? RFClockSyncStatus.Enabled : RFClockSyncStatus.Disabled;
            this.RFClockSignalLevelStatus = isRFLevelStrong ? RFClockSignalLevelStatus.Strong : RFClockSignalLevelStatus.Weak;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ClockData);
        }

        public bool Equals(ClockData other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return
                    Clock == other.Clock &&
                    TimeZoneOffset == other.TimeZoneOffset &&
                    PowerConnectorStatus == other.PowerConnectorStatus &&
                    BatteryLevelStatus == other.BatteryLevelStatus &&
                    RFClockSyncStatus == other.RFClockSyncStatus &&
                    RFClockSignalLevelStatus == other.RFClockSignalLevelStatus;
            }
        }
    }
}
