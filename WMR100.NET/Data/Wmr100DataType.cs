namespace WMR100.NET.Data
{
    public enum Wmr100DataType : byte
    {
        Unknown = 0x00,
        Rain = 0x41,
        TemperatureHumidity = 0x42,
        TemperatureWater = 0x44,
        Pressure = 0x46,
        UV = 0x47,
        Wind = 0x48,
        Clock = 0x60,
    }
}
