using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WMR100.NET.Helpers
{
    public static class ByteArrayUtils
    {
        public static string ByteArrayToString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        public static byte[] StringToByteArray(string hexString)
        {
            return Enumerable.Range(0, hexString.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                             .ToArray();
        }

        public static ICollection<byte[]> StringToByteArray(ICollection<string> hexStrings)
        {
            var bytes = new Collection<byte[]>();

            foreach (var hexString in hexStrings)
            {
                bytes.Add(StringToByteArray(hexString));
            };

            return bytes;
        }
    }
}
