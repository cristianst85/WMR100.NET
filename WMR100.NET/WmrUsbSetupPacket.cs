using LibUsbDotNet.Main;

namespace WMR100.NET
{
    internal static class WmrUsbSetupPacket
    {
        /// <summary>
        /// <para>Direction: Host-to-Device (0)</para>
        /// <para>Type: Class (0x1)</para>
        /// <para>Recipient: Interface (0x01)</para>
        /// </summary>
        private static readonly byte bmRequestType = 0x21;

        /// <summary>
        /// HID Request Type: Set Report (0x09).
        /// </summary>
        private static readonly byte bRequest = 0x09;

        /// <summary>
        /// Value.
        /// </summary>
        private static readonly short wValue = 0x0200;

        /// <summary>
        /// Index.
        /// </summary>
        private static readonly short wIndex = 0x0000;

        /// <summary>
        /// Number of bytes to transfer.
        /// </summary>
        private static readonly short wLength = 8;

        public static UsbSetupPacket Create()
        {
            return new UsbSetupPacket(bmRequestType, bRequest, wValue, wIndex, wLength);
        }
    }
}
