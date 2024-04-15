using NUnit.Framework;
using System;
using System.Collections;
using WMR100.NET.Helpers;
using WMR100.NET.SensorData;

namespace WMR100.NET.Tests
{
    [TestFixture]
    public class Wmr100DataTests
    {
        [Test, TestCaseSource("TestCases")]
        public void TryDecode(string packet, Wmr100Data expectedResult)
        {
            bool success = Wmr100Data.TryDecode(ByteArrayUtils.StringToByteArray(packet), out Wmr100Data wmr100Data);

            Console.WriteLine("Actual result: " + Newtonsoft.Json.JsonConvert.SerializeObject(wmr100Data));
            Console.WriteLine("Expected result: " + Newtonsoft.Json.JsonConvert.SerializeObject(expectedResult));

            Assert.That(success, Is.True);
            Assert.That(wmr100Data, Is.EqualTo(expectedResult));
        }

        private static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(
                    "406000001F080C061102EC00",
                    new ClockData(new DateTime(2017, 06, 12, 08, 31, 00, DateTimeKind.Local), 02, RFClockSyncStatus.Disabled, RFClockSignalLevelStatus.Weak, PowerConnectorStatus.NotConnected, BatteryLevelStatus.Low)
                ).SetDescription("Oregon Scientific RMS600");

                yield return new TestCaseData(
                    "4060000026150F0611FDFE01",
                    new ClockData(new DateTime(2017, 06, 15, 21, 38, 00, DateTimeKind.Local), -03, RFClockSyncStatus.Disabled, RFClockSignalLevelStatus.Weak, PowerConnectorStatus.NotConnected, BatteryLevelStatus.Low)
                ).SetDescription("Oregon Scientific RMS600");

                yield return new TestCaseData(
                    "4060000028010F0611E9D801",
                    new ClockData(new DateTime(2017, 06, 15, 01, 40, 00, DateTimeKind.Local), -23, RFClockSyncStatus.Disabled, RFClockSignalLevelStatus.Weak, PowerConnectorStatus.NotConnected, BatteryLevelStatus.Low)
                ).SetDescription("Oregon Scientific RMS600");

                yield return new TestCaseData(
                    "0042A237011956000030BB01",
                    new TemperatureHumidityData(2, 31.1m, TrendType.Steady, 25, TrendType.Falling, 8.6m, ComfortLevelType.Poor, null, BatteryLevelStatus.Normal)
                ).SetDescription("Oregon Scientific RMS600");

                yield return new TestCaseData(
                    "6042E343004A170000305902",
                    new TemperatureHumidityData(3, 6.7m, TrendType.Falling, 74, TrendType.Falling, 2.3m, ComfortLevelType.Fair, null, BatteryLevelStatus.Low)
                ).SetDescription("Oregon Scientific RMS600");

                yield return new TestCaseData(
                    "4042001D0124003000302401",
                    new TemperatureHumidityData(0, 28.5m, TrendType.Steady, 36, TrendType.Steady, 0.0m, ComfortLevelType.Unknown, null, BatteryLevelStatus.Low)
                ).SetDescription("Oregon Scientific RMS600");
                yield return new TestCaseData(
                    "5042034300321E800030D801",
                    new TemperatureHumidityData(3, 6.7m, TrendType.Rising, 50, TrendType.Steady, -3.0m, ComfortLevelType.Unknown, null, BatteryLevelStatus.Low)
                ).SetDescription("Oregon Scientific RMS600");

                yield return new TestCaseData(
                    "104241F70029680000304B02",
                    new TemperatureHumidityData(1, 24.7m, TrendType.Rising, 41, TrendType.Steady, 10.4m, ComfortLevelType.Good, null, BatteryLevelStatus.Normal)
                ).SetDescription("Oregon Scientific RMS600");

                yield return new TestCaseData(
                    "404200150123003000301B01",
                    new TemperatureHumidityData(0, 27.7m, TrendType.Steady, 35, TrendType.Steady, 0.0m, ComfortLevelType.Unknown, null, BatteryLevelStatus.Low)
                ).SetDescription("Oregon Scientific RMS600");

                yield return new TestCaseData(
                    "204202AC00313F000030B001",
                    new TemperatureHumidityData(2, 17.2m, TrendType.Falling, 49, TrendType.Steady, 6.3m, ComfortLevelType.Unknown, null, BatteryLevelStatus.Normal)
               ).SetDescription("Oregon Scientific RMS600");

                yield return new TestCaseData(
                    "1042D0D10062D20000204703",
                    new TemperatureHumidityData(0, 20.9m, TrendType.Rising, 98, TrendType.Rising, 21.0m, ComfortLevelType.Fair, null, BatteryLevelStatus.Normal)
                ).SetDescription("Decode_WMR100-200_v5.xlsm");

                yield return new TestCaseData(
                    "106000001512120408813601",
                    new ClockData(new DateTime(2008, 04, 18, 18, 21, 00, DateTimeKind.Local), -1, RFClockSyncStatus.Enabled, RFClockSignalLevelStatus.Weak, PowerConnectorStatus.NotConnected, BatteryLevelStatus.Normal)
               ).SetDescription("Decode_WMR100-200_v5.xlsm").Ignore("TODO: Need to figure out why this fails!");
            }
        }
    }
}
