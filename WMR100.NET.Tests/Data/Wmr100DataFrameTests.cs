using NUnit.Framework;
using WMR100.NET.Data;
using WMR100.NET.Helpers;

namespace WMR100.NET.Tests.Data
{

    [TestFixture]
    public class Wmr100DataFrameTests
    {
        [TestCase("FFFF40600000380204061102F700", true)]
        [TestCase("FFFF404200150129003000302101", true)]
        [TestCase("FFFF404200150129003000312101", false)]
        public void IsChecksumValid(string frame, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, new Wmr100DataFrame(ByteArrayUtils.StringToByteArray(frame)).IsChecksumValid());
        }

        [TestCase("FFFF40600000380204061102F700", true)]
        [TestCase("FFFF404200150129003000302101", true)]
        [TestCase("FFFF40420015012900300030002101", false)]
        public void IsLengthValid(string frame, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, new Wmr100DataFrame(ByteArrayUtils.StringToByteArray(frame)).IsLengthValid());
        }

        [TestCase("FFFF40600000380204061102F700", "40600000380204061102")]
        [TestCase("FFFF404200150129003000302101", "40420015012900300030")]
        public void GetPacketData(string frame, string packet)
        {
            Assert.AreEqual(ByteArrayUtils.StringToByteArray(packet), new Wmr100DataFrame(ByteArrayUtils.StringToByteArray(frame)).GetPacketData());
        }
    }
}
