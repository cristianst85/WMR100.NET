using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WMR100.NET.Helpers;

namespace WMR100.NET
{
    internal class Wmr100DataFrameAssembler
    {
        public static Action<string> Log;

        private readonly byte[] buffer = new byte[256];
        private int bufferPos = -1;

        public ICollection<Wmr100DataFrame> Assemble(byte[] usbDataBlock)
        {
            var dataFrames = new Collection<Wmr100DataFrame>();

            /// USB data is received in blocks that have a byte count and relevant data, 
            /// padded out to 8 bytes. Depending on the USB interface software, this may 
            /// be preceded by a report code byte (normally zero). The byte count indicates 
            /// how many of the following bytes should be used as data (e.g. a count of 2 
            /// means that only the following 2 bytes should be used). Input data from 
            /// the weather station arrives in bursts roughly every minute, using data it
            /// has previously accumulated.
            /// Source: http://www.cs.stir.ac.uk/~kjt/software/comms/wmr180.html
            /// or https://web.archive.org/web/20200107095622/https://www.cs.stir.ac.uk/~kjt/software/comms/wmr180.html

            Array.Copy(usbDataBlock, 1, buffer, bufferPos + 1, usbDataBlock[0]);
            bufferPos += usbDataBlock[0];

            /// The data from these USB blocks needs to be concatenated into one input stream. 
            /// Input frames are extracted from this stream, <seealso cref="Wmr100DataFrame"/>.

            while (true)
            {
                var dataFrameStartPos = -1;

                for (var i = 0; i < (bufferPos - 2); i++)
                {
                    if (buffer[i] == Wmr100DataFrame.Delimiter && buffer[i + 1] == Wmr100DataFrame.Delimiter)
                    {
                        dataFrameStartPos = i;
                        break;
                    }
                }

                if (dataFrameStartPos < 0)
                {
                    InternalLog("Not enough data available.");
                    break; // Internal buffer does not contain enough data to assemble a data frame.
                }

                if (dataFrameStartPos > 0)
                {
                    // Incomplete data frame at the beginning of the data stream.
                    InternalLog($"Ignored {dataFrameStartPos} byte(s) of data.");
                }

                var dataFrameEndPos = -1;

                for (var i = dataFrameStartPos + 2; i < (bufferPos - 2); i++)
                {
                    if (buffer[i] == Wmr100DataFrame.Delimiter && buffer[i + 1] == Wmr100DataFrame.Delimiter)
                    {
                        dataFrameEndPos = i;
                        break;
                    }
                }

                if (dataFrameEndPos < 0)
                {
                    InternalLog("Not enough data available.");
                    break; // Internal buffer does not contain enough data to assemble a data frame.
                }

                var len = dataFrameEndPos - dataFrameStartPos;

                if (len == 0)
                {
                    InternalLog("Ignored zero length data frame.");
                }
                else
                {
                    var data = new byte[len];
                    Array.Copy(buffer, dataFrameStartPos, data, 0, len);
                    InternalLog($"Found data frame: {ByteArrayUtils.ByteArrayToString(data)}");
                    dataFrames.Add(new Wmr100DataFrame(data));
                }

                Array.Copy(buffer, dataFrameEndPos, buffer, 0, bufferPos - dataFrameEndPos + 1);
                bufferPos -= dataFrameEndPos;
                Array.Clear(buffer, bufferPos + 1, buffer.Length - bufferPos - 1);
            }

            return dataFrames;
        }

        private static void InternalLog(string message)
        {
            Log?.Invoke(message);
        }
    }
}
