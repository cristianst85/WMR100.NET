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
        [SetUp]
        public void Setup()
        {
            Wmr100DataFrameAssembler.Log = (message) => { Debug.WriteLine(message); };
        }

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
                Debug.WriteLine(ByteArrayUtils.ByteArrayToString(frame.Data));
            }

            Debug.WriteLine("Expected frames:");

            foreach (var frame in expectedFrames)
            {
                Debug.WriteLine(frame);
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

                yield return new TestCaseData(
                   new Collection<string> {
                        "0701FFFF204281E4",
                        "07001F2F00003045",
                        "0702FFFF00420014",
                        "07011C00300030D3",
                        "0700FFFF00420013",
                        "07011C00300030D2",
                        "0700FFFF20422344",
                        "0700410600003040",
                        "0701FFFF0042820D",
                        "070119320000304D",
                        "0701FFFF00420013",
                        "07011C00300030D2",
                        "0700FFFF204281E4",
                        "07001F2F00003045",
                        "0702FFFF00420013",
                        "0701FFFF00420012",
                        "07011C00300030D1",
                        "0700FFFF00600000",
                        "0702001006180292",
                        "0700FFFF00420012",
                        "07011C00300030D1",
                        "0700FFFF20422344",
                        "0700410600003040",
                        "070100420013011C",
                        "0700300030D200FF",
                        "07FFFFFF00420013"
                   },
                   new Collection<string>
                   {
                       "FFFF204281E4001F2F0000304502",
                       "FFFF00420014011C00300030D300",
                       "FFFF00420013011C00300030D200",
                       "FFFF204223440041060000304001",
                       "FFFF0042820D0119320000304D01",
                       "FFFF00420013011C00300030D200",
                       "FFFF204281E4001F2F0000304502",
                       "FFFF0042001301",
                       "FFFF00420012011C00300030D100",
                       "FFFF006000000200100618029200",
                       "FFFF00420012011C00300030D100",
                       "FFFF20422344004106000030400100420013011C00300030D200",
                       "FFFF"
                   }
               );
            }
        }
    }
}
