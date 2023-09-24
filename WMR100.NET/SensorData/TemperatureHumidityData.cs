using System;

namespace WMR100.NET.SensorData
{
    public class TemperatureHumidityData : Wmr100SensorData, IEquatable<TemperatureHumidityData>
    {
        public override Wmr100DataType Type
        {
            get
            {
                return Wmr100DataType.TemperatureHumidity;
            }
        }

        public decimal Temperature { get; private set; }

        public TrendType TemperatureTrend { get; private set; }

        public decimal Humidity { get; private set; }

        public TrendType HumidityTrend { get; private set; }

        public decimal DewPoint { get; private set; }

        public ComfortLevelType ComfortLevel { get; private set; }

        public decimal? HeatIndex { get; private set; }

        public TemperatureHumidityData(int sensorId, decimal temperature, TrendType temperatureTrend, decimal humidity, TrendType humidityTrend, decimal dewPoint, ComfortLevelType comfortLevel, decimal? heatIndex, BatteryLevelStatus batteryLevelStatus)
        {
            this.SensorId = sensorId;
            this.Temperature = temperature;
            this.TemperatureTrend = temperatureTrend;
            this.Humidity = humidity;
            this.HumidityTrend = humidityTrend;
            this.DewPoint = dewPoint;
            this.ComfortLevel = comfortLevel;
            this.HeatIndex = heatIndex;
            this.BatteryLevelStatus = batteryLevelStatus;
        }

        public TemperatureHumidityData(int sensorId, decimal temperature, int temperatureTrend, decimal humidity, int humidityTrend, decimal dewPoint, int comfortLevel, decimal? heatIndex, bool hasLowBattery)
        {
            this.SensorId = sensorId;
            this.Temperature = temperature;
            this.TemperatureTrend = (TrendType)temperatureTrend;
            this.Humidity = humidity;
            this.HumidityTrend = (TrendType)humidityTrend;
            this.DewPoint = dewPoint;
            this.ComfortLevel = (ComfortLevelType)comfortLevel;
            this.HeatIndex = heatIndex;
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

            return Equals(obj as TemperatureHumidityData);
        }

        public bool Equals(TemperatureHumidityData other)
        {
            if (other == null)
            {
                return false;
            }

            return SensorId == other.SensorId &&
                Temperature == other.Temperature &&
                TemperatureTrend == other.TemperatureTrend &&
                Humidity == other.Humidity &&
                HumidityTrend == other.HumidityTrend &&
                DewPoint == other.DewPoint &&
                ComfortLevel == other.ComfortLevel &&
                HeatIndex == other.HeatIndex &&
                BatteryLevelStatus == other.BatteryLevelStatus;
        }
    }
}
