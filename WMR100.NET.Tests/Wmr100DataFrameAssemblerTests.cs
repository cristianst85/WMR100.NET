using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using WMR100.NET.Helpers;

namespace WMR100.NET.Tests
{
    [TestFixture]
    public class Wmr100DataFrameAssemblerTests
    {
        [Test, TestCaseSource("TestCases")]
        public void AssemblePacket(ICollection<string> usbDataBlocks, ICollection<string> expectedFrames)
        {
            var assembler = new Wmr100DataFrameAssembler();
            var frames = new List<Wmr100DataFrame>();

            foreach (var usbDataBlock in ByteArrayUtils.StringToByteArray(usbDataBlocks))
            {
                frames.AddRange(assembler.Assemble(usbDataBlock));
            }

            Debug.WriteLine("Resulted frames:");
            foreach (var frame in frames)
            {
                Debug.WriteLine(BitConverter.ToString(frame.Data).Replace("-", string.Empty));
            }

            Debug.WriteLine("Expected frames:");
            foreach (var frame in expectedFrames)
            {
                Debug.WriteLine(BitConverter.ToString(ByteArrayUtils.StringToByteArray(frame)).Replace("-", string.Empty));
            }

            Assert.That(frames.Select(x => x.Data), Is.EqualTo(ByteArrayUtils.StringToByteArray(expectedFrames)));
        }

        private static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new Collection<string> { }, new Collection<string> { });

                yield return new TestCaseData(
                    new Collection<string> {
                        "0629003000302100",
                        "01FF3041026F0029",
                        "06FF406000003829",
                        "070204061102F700",
                        "01FF04061102F700",
                        "05FF40420015F700",
                        "0601290030003000",
                        "0221010030003000",
                        "01FF010030003000",
                        "06FF404200150100"
                    },
                    new Collection<string>
                    {
                        "FFFF40600000380204061102F700",
                        "FFFF404200150129003000302101"
                    }
                );
            }
        }
    }
}
