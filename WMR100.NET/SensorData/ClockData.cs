using System;

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

        public RFClockSyncStatus RFClockSyncStatus { get; private set; }

        public RFClockSignalLevelStatus RFClockSignalLevelStatus { get; private set; }

        public PowerConnectorStatus PowerConnectorStatus { get; private set; }

        public BatteryLevelStatus BatteryLevelStatus { get; private set; }

        public ClockData(DateTime clock, int timeZoneOffset, RFClockSyncStatus rfClockSyncStatus, RFClockSignalLevelStatus rfClockSignalLevelStatus, PowerConnectorStatus powerConnectorStatus, BatteryLevelStatus batteryLevelStatus)
        {
            this.Clock = clock;
            this.TimeZoneOffset = timeZoneOffset;
            this.RFClockSyncStatus = rfClockSyncStatus;
            this.RFClockSignalLevelStatus = rfClockSignalLevelStatus;
            this.PowerConnectorStatus = powerConnectorStatus;
            this.BatteryLevelStatus = batteryLevelStatus;
        }

        public ClockData(DateTime clock, int timeZoneOffset, bool isRFSyncEnabled, bool isRFLevelStrong, bool isPowered, bool hasLowBattery)
        {
            this.Clock = clock;
            this.TimeZoneOffset = timeZoneOffset;
            this.RFClockSyncStatus = isRFSyncEnabled ? RFClockSyncStatus.Enabled : RFClockSyncStatus.Disabled;
            this.RFClockSignalLevelStatus = isRFLevelStrong ? RFClockSignalLevelStatus.Strong : RFClockSignalLevelStatus.Weak;
            this.PowerConnectorStatus = isPowered ? PowerConnectorStatus.Connected : PowerConnectorStatus.NotConnected;
            this.BatteryLevelStatus = hasLowBattery ? BatteryLevelStatus.Low : BatteryLevelStatus.Normal;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as ClockData);
        }

        public bool Equals(ClockData other)
        {
            if (other == null)
            {
                return false;
            }

            return Clock == other.Clock &&
                TimeZoneOffset == other.TimeZoneOffset &&
                RFClockSyncStatus == other.RFClockSyncStatus &&
                RFClockSignalLevelStatus == other.RFClockSignalLevelStatus &&
                PowerConnectorStatus == other.PowerConnectorStatus &&
                BatteryLevelStatus == other.BatteryLevelStatus;
        }
    }
}
